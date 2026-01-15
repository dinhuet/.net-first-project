using System.Security.Cryptography;
using AutoMapper;
using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IRepositories;
using BlogApp.Application.IServices;
using BlogApp.Application.MiddleWare;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.ExternalServices.Interface;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpRepository _otpRepository;
    private readonly IEmailService _emailService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IMapper _map;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    
    public AuthService(IUserRepository userRepository,  IMapper map,  ITokenService tokenService
        , IRefreshTokenRepository refreshTokenRepository, IOtpRepository otpRepository, IEmailService emailService)
    {
        _emailService = emailService;
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _map = map;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public void Register(RegisterRequestDto register)
    {
        if (register.ConfirmPassword != register.Password)  
        {
            throw new AppException(ErrorCode.ConfirmPasswordIsNotMatch);
        }
        
        User newUser = _map.Map<User>(register);

        newUser.Role = UserRole.User;
        newUser.Password = _passwordHasher.HashPassword(newUser, register.Password);
        
        _userRepository.Register(newUser);
    }

    public AuthResponseDto Login(LoginRequestDto login)
    {
        User user = _userRepository.GetUserByEmail(login.Email);
        if (user is null)
        {
            throw new AppException(ErrorCode.UserNotFound); 
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);
        
        if (result != PasswordVerificationResult.Success)
        {
            throw new AppException(ErrorCode.PasswordIsNotMatch);
        }
        
        var token = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);
                
        return new AuthResponseDto(token, refreshToken.Token);
    }

    public void Logout(LogoutRequestDto logout)
    {
        var token = _refreshTokenRepository.GetRefreshToken(logout.RefreshToken);
        if (token is null)
        {
            throw new AppException(ErrorCode.NotAuthenticate);
        }
        
        _refreshTokenRepository.DeleteRefreshToken(token);
    }

    public async Task SendOtp(string email)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user is null) 
        {
            throw new AppException(ErrorCode.UserNotFound);
        }
        
        var oldOtp = _otpRepository.GetValidOtp(email);
        if (oldOtp is not null)
        {
            oldOtp.IsUsed = true;
            _otpRepository.UpdateOtp(oldOtp);
        }
        
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);

        var resetOtp = new Otp
        {
            UserId = user.Id,
            OtpHash = otpHash,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };
         _otpRepository.AddOtp(resetOtp);
        
        await _emailService.SendAsync(
            user.Email,
            "Reset Password OTP",
            $"Your OTP: {otp}"
        );
    }

    public void CheckOtp(VerifyOtpRequestDto verifyOtpRequestDto)
    {
        var otp = _otpRepository.GetValidOtp(verifyOtpRequestDto.Email);

        if (otp is null)
        {
            throw new AppException(ErrorCode.OtpIsInvalid);
        }
        var isValid = BCrypt.Net.BCrypt.Verify(verifyOtpRequestDto.Otp, otp.OtpHash);

        if (!isValid)
        {
            throw new AppException(ErrorCode.OtpIsNotTrue);
        }
        
        otp.IsUsed = true;
        _otpRepository.UpdateOtp(otp);
    }

    public void UpdatePassword(UpdatePasswordRequestDto dto, string email)
    {
        var  user = _userRepository.GetUserByEmail(email);
        if (dto.OldPassword is not null)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
        
            if (result != PasswordVerificationResult.Success)
            {
                throw new AppException(ErrorCode.PasswordIsNotMatch);
            }
        }

        if (dto.ConfirmPassword != dto.Password)
            throw new AppException(ErrorCode.ConfirmPasswordIsNotMatch);
        
        user.Password = _passwordHasher.HashPassword(user, dto.Password);
        _userRepository.UpdateUser(user);
    }
}