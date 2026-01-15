namespace BlogApp.Application.DTO.Request.Authenticate;

public class UploadImageRequestDto
{
    public IFormFile Image { get; set; } = null!;
}