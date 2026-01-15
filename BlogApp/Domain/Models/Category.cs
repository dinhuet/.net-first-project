using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Domain.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Category's name is required")]
    [MaxLength(100, ErrorMessage = "Category's name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category's slug is required")]
    [MaxLength(100, ErrorMessage = "Category's slug cannot exceed 100 characters")]
    public string Slug { get; set; } = string.Empty;

    public ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
}