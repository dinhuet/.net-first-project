namespace BlogApp.Application.DTO.Response;

public record AuthResponseDto(
    string Token,
    string RefreshToken
);