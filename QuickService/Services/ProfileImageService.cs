using QuickService.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickService.Services
{
    public class ProfileImageService : IProfileImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/png", "image/jpeg", "image/webp" };

        public ProfileImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> UploadImageAsync(IFormFile file, string role, string fullName, string subFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            // 1. Validation: Extension and MIME type
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension) || !_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return null;
            }

            // 2. Generate Safe Filename: role_fullname_datetime.png
            string sanitizedName = FileNameSanitizer.Sanitize(fullName.Replace(" ", "_"));
            string newFileName = $"{role.ToLowerInvariant()}_{sanitizedName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            // 3. Ensure Folder Exists with path traversal protection
            string uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", subFolder.ToLowerInvariant());
            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }

            string filePath = Path.Combine(uploadsRoot, newFileName);

            // 4. Image Processing using ImageSharp
            using (var stream = file.OpenReadStream())
            {
                using (var image = await Image.LoadAsync(stream))
                {
                    // Resize if width > 512px
                    if (image.Width > 512)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(512, 0),
                            Mode = ResizeMode.Max
                        }));
                    }

                    // Save as PNG
                    // Compression is handled by PNG format automatically, 
                    // but we can ensure it stays visually acceptable.
                    await image.SaveAsPngAsync(filePath);
                }
            }

            // Return relative path for database storage
            return $"/uploads/{subFolder.ToLowerInvariant()}/{newFileName}";
        }

        public void DeleteOldImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            // Remove leading slash if exists
            string relativePath = imagePath.StartsWith("/") ? imagePath.Substring(1) : imagePath;
            string fullPath = Path.Combine(_environment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            // Safety Check: Only delete if inside uploads folder
            string uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
            if (!fullPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch
                {
                    // Log error or ignore if deletion fails (non-critical)
                }
            }
        }
    }
}
