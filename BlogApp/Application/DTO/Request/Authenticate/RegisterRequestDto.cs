using System.ComponentModel.DataAnnotations;
using BlogApp.Domain.Enums;

namespace BlogApp.Application.DTO.Request.Authenticate;

public record RegisterRequestDto
(
    [MaxLength(200, ErrorMessage = "First name cannot exceed 200 characters")]
    string FirstName,

    [Required(ErrorMessage = "Last name must not be empty")]
    [MaxLength(200, ErrorMessage = "Last name cannot exceed 200 characters")]
    string LastName,
    
    [Required(ErrorMessage = "User name must not be empty")]
    [MaxLength(200, ErrorMessage = "User name cannot exceed 200 characters")]
    string UserName,
    
    [Required(ErrorMessage = "Email must not be empty")]
    [EmailAddress(ErrorMessage = "Email has invalid format")]
    string Email,

    [Required(ErrorMessage = "Password must not be empty")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
    string Password,
    
    [Required(ErrorMessage = "ConfirmPassword must not be empty")]
    string ConfirmPassword
    );