using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class Like
{
    [Required(ErrorMessage = "BlogId is required")]
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;

    [Required(ErrorMessage = "UserId is required")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}