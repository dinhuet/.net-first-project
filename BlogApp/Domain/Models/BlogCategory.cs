using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class BlogCategory
{
    [Required(ErrorMessage = "BlogId is required")]
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;

    [Required(ErrorMessage = "TagId is required")]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}