using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class BlogTag
{
    [Required(ErrorMessage = "BlogId is required")]
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;

    [Required(ErrorMessage = "TagId is required")]
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}