namespace BlogApp.Application.DTO.Request.Authenticate;

public record UserProfileRequestDto
(
    string? FirstName,
    string? LastName,
    string? UserName,
    string? Email
    );