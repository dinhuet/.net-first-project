namespace BlogApp.Application.DTO.Request.Authenticate;

public record VerifyOtpRequestDto(
    string Email,
    string Otp
    );