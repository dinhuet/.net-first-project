using BlogApp.Domain.Models;

namespace BlogApp.Application.IRepositories;

public interface IRefreshTokenRepository
{
    public void AddRefreshToken(RefreshToken token);
    public void DeleteRefreshToken(RefreshToken token);
    public RefreshToken GetRefreshToken(string refreshToken);
}