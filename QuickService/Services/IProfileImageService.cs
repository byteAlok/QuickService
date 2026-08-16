using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace QuickService.Services
{
    public interface IProfileImageService
    {
        Task<string?> UploadImageAsync(IFormFile file, string role, string fullName, string subFolder);
        void DeleteOldImage(string? imagePath);
    }
}
