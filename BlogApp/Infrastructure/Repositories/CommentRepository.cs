using BlogApp.Application.IRepositories;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<CommentRepository> _logger;
    public CommentRepository(AppDbContext db,  ILogger<CommentRepository> logger)
    {
        _logger = logger;
        _db = db;
        _logger.LogInformation("commentRepo created: " + _db.GetHashCode());
    }
    
    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        Console.WriteLine("comment repo: " + _db.GetHashCode());

        _db.Comments.Add(comment);
        return comment;
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _db.Comments
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
}