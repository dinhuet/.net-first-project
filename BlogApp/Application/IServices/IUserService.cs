using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Response;

namespace BlogApp.Application.IServices;

public interface IUserService
{ 
   UserProfileResponseDto UpdateProfile(UserProfileRequestDto userProfileRequestDto, string email);
   Task<string?> UpdateAvatarAsync(IFormFile file, string email);
   UserProfileResponseDto GetProfile(string email);
}