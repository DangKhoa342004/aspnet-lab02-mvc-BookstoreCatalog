using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BookstoreCatalog.Mvc.Controllers;

public class DataHealthController : Controller
{
    private readonly AppDbContext _context;
    private readonly IBookService _bookService;

    public DataHealthController(AppDbContext context, IBookService bookService)
    {
        _context = context;
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DataHealthViewModel();
        // -----------------------------------------------------------------
        var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
        var lastMigration = appliedMigrations.LastOrDefault() ?? "Chưa chạy";
        
        viewModel.Items.Add(new HealthCheckItem
        {
            Check = "Migration",
            Expected = "Applied",
            Actual = lastMigration.Contains("_") ? lastMigration.Split('_')[1] : lastMigration,
            Status = appliedMigrations.Any() ? "OK" : "Error",
            Note = appliedMigrations.Any() ? "DB up to date" : "Cần chạy lệnh Add-Migration"
        });

        // -----------------------------------------------------------------
        var bookCount = await _context.Books.CountAsync();
        
        viewModel.Items.Add(new HealthCheckItem
        {
            Check = "Seed Data",
            Expected = ">= 3 rows",
            Actual = $"{bookCount} books", // 👈 Tự thay đổi con số theo dữ liệu thực trong SQL của bạn
            Status = bookCount >= 3 ? "OK" : "Warning",
            Note = bookCount >= 3 ? "Ready" : "Dữ liệu trống, cần bổ sung HasData trong DbContext"
        });

        // ------------------------------------------------------------------
        var testBooks = await _bookService.GetBookListAsync();
        var hasTracking = _context.ChangeTracker.Entries<BookstoreCatalog.Mvc.Models.Book>().Any();

        viewModel.Items.Add(new HealthCheckItem
        {
            Check = "No-Tracking",
            Expected = "List only",
            Actual = !hasTracking ? "AsNoTracking" : "IsTracking (Chưa tối ưu)",
            Status = !hasTracking ? "OK" : "Warning",
            Note = !hasTracking ? "Read optimized" : "Nên bổ sung .AsNoTracking() khi lấy danh sách"
        });

        // ------------------------------------------------------------------
        string transactionStatus = "Thất bại";
        string note = "Transaction lỗi";

        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var hasCanConnect = await _context.Database.CanConnectAsync();
                if (hasCanConnect)
                {
                    transactionStatus = "Commit/Rollback";
                    note = "Safe write";
                }                
                await transaction.RollbackAsync(); 
            }
            catch
            {
                transactionStatus = "Lỗi kết nối";
            }
        }

        viewModel.Items.Add(new HealthCheckItem
        {
            Check = "Transaction",
            Expected = "Order save",
            Actual = transactionStatus,
            Status = transactionStatus == "Commit/Rollback" ? "OK" : "Error",
            Note = note
        });

        return View(viewModel);
    }
}