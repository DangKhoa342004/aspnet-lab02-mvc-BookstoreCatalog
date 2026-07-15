using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Services;
public interface IFileUploadService
{
    Task<string> SaveBookImageAsync(IFormFile file);
}