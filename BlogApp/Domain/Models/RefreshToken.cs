using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class RefreshToken
{
    public int Id { get; set; }
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public DateTime Expires { get; set; }
    
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }
}