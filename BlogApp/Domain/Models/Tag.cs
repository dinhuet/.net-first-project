using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tag's name is required")]
    [MaxLength(100, ErrorMessage = "Tag's name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tag's slug is required")]
    [MaxLength(100, ErrorMessage = "Tag's slug cannot exceed 100 characters")]
    public string Slug { get; set; } = string.Empty;

    public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
}