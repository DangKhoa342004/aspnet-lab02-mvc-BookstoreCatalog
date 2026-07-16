using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace BookstoreCatalog.Mvc.Controllers;

[ApiController]
[Route("api/books")]
[Authorize(Policy = "CanViewDataHealth")]
public class ApiBooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly AppDbContext _context;

    public ApiBooksController(IBookService bookService, AppDbContext context)
    {
        _bookService = bookService;
        _context = context;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword) || keyword.Length > 50)
        {
            var validationDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Dữ liệu đầu vào không hợp lệ",
                Instance = HttpContext.Request.Path
            };

            if (string.IsNullOrWhiteSpace(keyword))
            {
                validationDetails.Errors.Add("keyword", new[] { "Từ khóa tìm kiếm không được để trống." });
            }
            else
            {
                validationDetails.Errors.Add("keyword", new[] { "Từ khóa không được vượt quá 50 ký tự." });
            }

            return BadRequest(validationDetails);
        }

        var results = await _context.Books.AsNoTracking()
            .Where(b => b.Title.Contains(keyword) || b.ISBN.Contains(keyword))
            .Select(b => new 
            {
                b.Id,
                b.Title,
                b.ISBN,
                b.Author,
                b.Price,
                b.Quantity
            }).ToListAsync();

        if (!results.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Không tìm thấy dữ liệu",
                Detail = $"Không có sách nào khớp với từ khóa '{keyword}'.",
                Instance = HttpContext.Request.Path
            };
                
            problemDetails.Extensions["errorCode"] = "BOOK_NOT_FOUND";
            problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return NotFound(problemDetails);
        }

        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book == null)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "/problems/book-not-found",
                Title = "Book not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The book with id {id} was not found.",
                Instance = HttpContext.Request.Path
            };
            problemDetails.Extensions.Add("errorCode", "BOOK_NOT_FOUND");
            return NotFound(problemDetails);
        }
        
        var result = new
        {
            book.Id,
            book.Title,
            book.ISBN,
            book.Author,
            book.Price,
            book.Quantity,
            GenreName = book.Genre?.Name
        };

        return Ok(result);
    }
}