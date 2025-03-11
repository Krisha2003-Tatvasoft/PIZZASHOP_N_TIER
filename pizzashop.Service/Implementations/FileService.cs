using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class FileService :IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        var allowedMimeTypes = new List<string> { "image/jpeg", "image/png", "image/webp" };

        // Allowed image extensions
        var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".webp" };

        string fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedMimeTypes.Contains(file.ContentType.ToLower()) || !allowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException("Invalid file type. Only image files (JPG, PNG, GIF, BMP, WEBP) are allowed.");
        }


        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
        Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return uniqueFileName;
    }
}
