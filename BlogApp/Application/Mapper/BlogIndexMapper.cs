using BlogApp.Application.Index;
using BlogApp.Domain.Models;

namespace BlogApp.Application.Mapper;

public static class BlogIndexMapper
{
    public static BlogIndex ToIndex(this Blog blog)
    {
        return new BlogIndex
        {
            Id = blog.Id,
            Title = blog.Title,
            Content = blog.Content,
            PublishedAt = blog.PublishedAt
        };
    }
}
