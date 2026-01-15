using System.Security.Claims;
using BlogApp.Application.DTO.Request.Comment;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IServices;
using BlogApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CommentResponseDto>>> CreateComment([FromBody] CommentRequestDto dto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var response = new ApiResponse<object>
        {
            Status = 200,
            Message = "Create comment successful",
            Data = await  _commentService.CreateComment(dto, email)
        };
        
        return Ok(response);
    }

}