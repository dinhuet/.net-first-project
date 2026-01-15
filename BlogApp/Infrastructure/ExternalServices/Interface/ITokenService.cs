using BlogApp.Domain.Models;

namespace BlogApp.Infrastructure.ExternalServices.Interface;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshTokenUri(int size = 64);
    RefreshToken GenerateRefreshToken(User user);
}