using System.Security.Claims;
using BlogApp.Application.DTO.Page;
using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Request.Blog;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.Index;
using BlogApp.Application.IServices;
using BlogApp.Infrastructure.ExternalServices.Impl;
using BlogApp.Infrastructure.ExternalServices.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogController : ControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly IBlogService _blogService;
    private readonly IBlogSuggestService _blogSuggestService;
    public BlogController(IUploadService uploadService,  IBlogService blogService,  IBlogSuggestService blogSuggestService)
    {
        _blogSuggestService = blogSuggestService;
        _blogService = blogService;
        _uploadService = uploadService;
    }
    
    [HttpPost("/image/upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage([FromForm] UploadImageRequestDto image)
    {
        var imageUrl = await _uploadService.UploadImageAsync(image.Image);
        var response = new ApiResponse<object>
        {
            Status = 200,
            Message = "Upload image successful",
            Data = imageUrl
        };
        
        return Ok(response);
    }

    [HttpPost("/create")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<string>>> CreateBlog([FromForm] CreateBlogRequestDto blog)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        await _blogService.CreateAsync(blog, email);
        var response = new ApiResponse<object>
        {
            Status = 200,
            Message = "Create blog successful",
        };
        
        return Ok(response);
    }
    
    [HttpGet("/get")]
    public ActionResult<ApiResponse<PagedResult<BlogResponseDto>>> GetPageProduct([FromQuery] BlogQueryDto dto)
    {
        var response = new ApiResponse<PagedResult<BlogResponseDto>>
        {
            Status = 200,
            Message = "Success",
            Data = _blogService.GetPageBlog(dto)
        };
        
        return  Ok(response);
    }

    [HttpPatch("publish")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> PublishBlog([FromQuery] int id)
    {
        await _blogService.PublishBlog(id);
        
        var response = new ApiResponse<object>
        {
            Status = 200,
            Message = "Publish blog successful",
        };
        
        return Ok(response);
    }

    [HttpGet("get/my")]
    [Authorize]
    public ActionResult<ApiResponse<PagedResult<BlogResponseDto>>> GetMyBlog([FromQuery] BlogQueryDto dto)
    {
        var email =  User.FindFirstValue(ClaimTypes.Email);
        var response = new ApiResponse<PagedResult<BlogResponseDto>>
        {
            Status = 200,
            Message = "Publish blog successful",
            Data = _blogService.GetMyBlog(email, dto)
        };
        
        return Ok(response);
    }
    
    // search engine
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<BlogIndex>>>> Search([FromQuery] string q)
    {
        var response = new ApiResponse<List<BlogIndex>>
        {
            Status = 200,
            Message = "Search blog successful",
            Data = await _blogSuggestService.SearchAsync(q)
        };
        
        return Ok(response); 
    }

    // suggest
    [HttpGet("suggest")]
    public async Task<ActionResult<ApiResponse<List<string>>>> Suggest([FromQuery] string q)
    {
        var response = new ApiResponse<List<string>>
        {
            Status = 200,
            Message = "Autocomplete successful",
            Data = await _blogSuggestService.SuggestAsync(q)
        };
        
        return Ok(response);
    }
}