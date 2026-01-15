using System.Security.Claims;
using BlogApp.Application.DTO.Request;
using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [Authorize]
    public ActionResult<ApiResponse<UserProfileResponseDto>> GetUserProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var response = new ApiResponse<UserProfileResponseDto>
        {
            Status = 200,
            Message = "Register successful",
            Data = _userService.GetProfile(email)
        };
        
        return Ok(response);
    }
    
    
    [HttpPatch("update/profile")]
    [Authorize]
    public ActionResult<ApiResponse<UserProfileResponseDto>> UpdateUserProfile([FromBody] UserProfileRequestDto userProfileRequestDto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var response = new ApiResponse<UserProfileResponseDto>
        {
            Status = 200,
            Message = "Update  profile successful",
            Data = _userService.UpdateProfile(userProfileRequestDto, email)
        };
        
        return Ok(response);
    }
    
    [HttpPut("update/avatar")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateAvatar( [FromForm] UpdateAvatarRequestDto avatar)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var result = await _userService.UpdateAvatarAsync(avatar.AvatarFile, email);

        var response = new ApiResponse<string>
        {
            Status = 200,
            Message = "Update avatar successful",
            Data = result
        };

        return Ok(response);
    }
}