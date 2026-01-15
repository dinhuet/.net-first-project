using BlogApp.Domain.Models;

namespace BlogApp.Application.IRepositories;

public interface IUserRepository
{
    void Register(User register);
    User GetUserByEmail(string email);
    Task<User> GetUserByEmailAsync(string email);
    Task<int> GetUserIdByEmailAsync(string email);
    void UpdateUser(User user);
}   