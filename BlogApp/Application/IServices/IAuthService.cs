using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Response;

namespace BlogApp.Application.IServices;

public interface IAuthService
{
    void Register(RegisterRequestDto register);
    AuthResponseDto Login(LoginRequestDto login);
   void Logout(LogoutRequestDto logout);
   Task SendOtp(string email);
   void CheckOtp(VerifyOtpRequestDto verifyOtpRequestDto);
   void UpdatePassword(UpdatePasswordRequestDto updatePasswordRequestDto, string email);
}