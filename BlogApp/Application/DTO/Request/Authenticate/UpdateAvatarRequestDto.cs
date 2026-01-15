
namespace BlogApp.Application.DTO.Request.Authenticate;

public class UpdateAvatarRequestDto
{
    public IFormFile AvatarFile { get; set; } = null!;
}