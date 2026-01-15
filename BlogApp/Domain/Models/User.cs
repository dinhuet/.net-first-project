using System.ComponentModel.DataAnnotations;
using BlogApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Domain.Models;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "UserName must not be empty")]
    [MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email must not be empty")]
    [EmailAddress(ErrorMessage = "Email is invalid format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password must not be empty")]
    public string Password { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name must not be empty")]
    [MaxLength(200)]
    public string LastName { get; set; } = string.Empty;
    public string? Avatar { get; set; }

    [Required]
    public UserRole Role { get; set; }
    
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public List<Otp> Otps { get; set; } = new List<Otp>();
}