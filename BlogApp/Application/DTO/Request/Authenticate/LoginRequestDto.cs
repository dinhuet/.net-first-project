using System.ComponentModel.DataAnnotations;

namespace BlogApp.Application.DTO.Request.Authenticate;

public record LoginRequestDto
(
    [Required(ErrorMessage = "Email must not be empty")]
    [EmailAddress(ErrorMessage = "Email has invalid format")]
    string Email,

    [Required(ErrorMessage = "Password must not be empty")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
    string Password
    );