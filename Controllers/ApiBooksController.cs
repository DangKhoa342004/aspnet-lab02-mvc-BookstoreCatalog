using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;

namespace BookstoreCatalog.Mvc.Controllers;

[ApiController]
[Route("api/books")]
[Authorize(Policy = "CanViewDataHealth")]
public class ApiBooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public ApiBooksController(IBookService bookService)
    {
        _bookService = bookService;
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
        return Ok(book);
    }
}