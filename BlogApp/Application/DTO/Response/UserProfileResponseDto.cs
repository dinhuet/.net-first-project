namespace BlogApp.Application.DTO.Response;

public record UserProfileResponseDto
(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Avatar
);