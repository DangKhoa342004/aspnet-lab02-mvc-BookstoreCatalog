using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Services;
public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveBookImageAsync(IFormFile file)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext)) 
            throw new InvalidOperationException("File type is not allowed. (Only allow .jpeg, .jpg, .png, .webp).");

        if (file.Length > 2 * 1024 * 1024) 
            throw new InvalidOperationException("File ảnh quá lớn, tối đa chỉ được 2MB.");

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "books");    
        Directory.CreateDirectory(folder);    
        var path = Path.Combine(folder, safeName);

        using var stream = new FileStream(path, FileMode.CreateNew);
        await file.CopyToAsync(stream);
        return $"/uploads/books/{safeName}";
    }
}