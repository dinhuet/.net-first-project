using System.ComponentModel.DataAnnotations;
using BlogApp.Domain.Enums;

namespace BlogApp.Application.DTO.Request.Blog;

public class CreateBlogRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    public BlogStatus Status { get; set; }
    
    public IFormFile? Avatar { get; set; }

    public List<int>? Categories { get; set; }
    public List<string>? TagNames { get; set; }
}
