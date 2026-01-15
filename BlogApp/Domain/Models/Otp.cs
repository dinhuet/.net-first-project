using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Models;

public class Otp
{
    public int Id { get; set; }

    [Required]
    public string OtpHash { get; set; } = null!;
    
    [Required]
    public DateTime ExpiredAt { get; set; }
    
    [Required]
    public int FailedAttempts { get; set; } = 0; 
    
    [Required]
    public bool IsUsed { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}