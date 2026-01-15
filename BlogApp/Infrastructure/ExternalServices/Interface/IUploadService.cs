namespace BlogApp.Infrastructure.ExternalServices.Interface;

public interface IUploadService
{
    void ValidateImage(IFormFile file);
    Task<string?> UploadImageAsync(IFormFile file);
    
}