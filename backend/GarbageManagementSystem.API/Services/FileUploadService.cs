using GarbageManagementSystem.API.Common;
using Microsoft.AspNetCore.Http;

namespace GarbageManagementSystem.API.Services;

public interface IFileUploadService
{
    /// <summary>
    /// Saves an uploaded image under wwwroot/uploads/{subFolder} and returns a relative
    /// path (e.g. "/uploads/requests/guid.jpg") suitable for storing in the database.
    /// Returns null if no file was supplied.
    /// </summary>
    Task<string?> SaveImageAsync(IFormFile? file, string subFolder);

    void DeleteImage(string? relativePath);
}

public class FileUploadService : IFileUploadService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> SaveImageAsync(IFormFile? file, string subFolder)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new BadRequestException("Only image files (jpg, jpeg, png, gif, webp) are allowed.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new BadRequestException("Image size must not exceed 5 MB.");
        }

        var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsFolder = Path.Combine(webRoot, "uploads", subFolder);
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{subFolder}/{fileName}";
    }

    public void DeleteImage(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var fullPath = Path.Combine(webRoot, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
