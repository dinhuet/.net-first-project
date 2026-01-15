using System.Security.Claims;
using AutoMapper;
using BlogApp.Application.DTO.Request;
using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IRepositories;
using BlogApp.Application.IServices;
using BlogApp.Infrastructure.ExternalServices.Interface;

namespace BlogApp.Application.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _map;
    private readonly IUploadService _uploadService;
    public UserService(IUserRepository userRepository, IMapper map,  IUploadService uploadService)
    {
        _uploadService = uploadService;
        _map = map;
        _userRepository = userRepository;
    }
    
    public UserProfileResponseDto UpdateProfile(UserProfileRequestDto dto, string email)
    {
        var user = _userRepository.GetUserByEmail(email);

        if (dto.Email != null) user.Email = dto.Email;
        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.UserName != null) user.UserName = dto.UserName;
        
         _userRepository.UpdateUser(user);
        
        return _map.Map<UserProfileResponseDto>(user);
    }

    public async Task<string?> UpdateAvatarAsync(IFormFile file, string email)
    {
        var user = _userRepository.GetUserByEmail(email);
        
        var avatarUrl = await _uploadService.UploadImageAsync(file);
        
        user.Avatar = avatarUrl;
         _userRepository.UpdateUser(user);
        
        return avatarUrl;
    }

    public UserProfileResponseDto GetProfile(string email)
    {
        return _map.Map<UserProfileResponseDto>(_userRepository.GetUserByEmail(email));
    }
}