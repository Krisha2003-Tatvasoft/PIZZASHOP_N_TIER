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
