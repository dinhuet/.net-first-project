using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "BlogId is required")]
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;

    [Required(ErrorMessage = "UserId is required")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    public int TotalReplies { get; set; } = 0;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Date is required")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}