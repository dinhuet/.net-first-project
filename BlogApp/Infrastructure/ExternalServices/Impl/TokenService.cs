using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogApp.Application.IRepositories;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.ExternalServices.Interface;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Infrastructure.ExternalServices.Impl;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(IConfiguration config,  IRefreshTokenRepository refreshTokenRepositoryRepository)
    {
        _refreshTokenRepository = refreshTokenRepositoryRepository;
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshTokenUri(int size = 64)
    {
        var randomNumber = new byte[size];
        using var rng = RandomNumberGenerator.Create(); 
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        RefreshToken token = new RefreshToken();
        token.Token = GenerateRefreshTokenUri();
        token.Expires = DateTime.UtcNow.AddDays(7);
        token.User = user;

        _refreshTokenRepository.AddRefreshToken(token);
        return token;
    }
    
}