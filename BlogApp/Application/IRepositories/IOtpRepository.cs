using BlogApp.Domain.Models;

namespace BlogApp.Application.IRepositories;

public interface IOtpRepository
{
    void AddOtp(Otp otp);
    Otp GetValidOtp(string email);
    void UpdateOtp(Otp otp);
}