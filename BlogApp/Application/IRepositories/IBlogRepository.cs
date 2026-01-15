using BlogApp.Application.DTO.Page;
using BlogApp.Application.Index;
using BlogApp.Domain.Models;

namespace BlogApp.Application.IRepositories;

public interface IBlogRepository
{
    Task AddAsync(Blog blog);
    IQueryable<Blog> GetPageBlog();
    Task PublishBlog(int id);
    Task<List<BlogIndex>> SearchBlogsByTitleAsync(string titleQuery);
}