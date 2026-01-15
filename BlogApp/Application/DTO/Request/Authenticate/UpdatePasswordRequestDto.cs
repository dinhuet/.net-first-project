using System.ComponentModel.DataAnnotations;

namespace BlogApp.Application.DTO.Request.Authenticate;

public record UpdatePasswordRequestDto
( 
    string? OldPassword,
    
    [Required]
    string Password,
    [Required]
    string ConfirmPassword
    );