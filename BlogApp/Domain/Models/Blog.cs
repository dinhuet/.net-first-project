using System.ComponentModel.DataAnnotations;
using BlogApp.Domain.Enums;

namespace BlogApp.Domain.Models;

public class Blog
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]   
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required")]
    public BlogStatus Status { get; set; }

    [Required(ErrorMessage = "Author is required")]
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    
    public string Avatar { get; set; } 

    public DateTime? PublishedAt { get; set; } = null!;
    
    [Required(ErrorMessage = "Created time is required")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
    public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}