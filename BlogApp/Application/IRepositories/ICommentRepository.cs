using BlogApp.Domain.Models;

namespace BlogApp.Application.IRepositories;

public interface ICommentRepository
{
    Task<Comment> AddCommentAsync(Comment comment);
    Task<Comment?> GetByIdAsync(int id);
}