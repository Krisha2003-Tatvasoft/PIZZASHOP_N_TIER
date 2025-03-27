using Microsoft.AspNetCore.Http;

namespace pizzashop.Service.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);

    bool DeleteFile(string filePath);
}
