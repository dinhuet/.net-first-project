using BlogApp.Application.DTO.Request.Category;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IServices;
using BlogApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<object>> CreateCategory([FromBody] CreateCategoryRequestDto category)
    {
        _categoryService.AddCategory(category);
        var response = new ApiResponse<object>
        {
            Status = 200,
            Message = "Create category successful",
        };
        
        return Ok(response);
    }
    
    [HttpGet]
    public ActionResult<ApiResponse<List<CategoryResponseDto>>>  GetCategories()
    {
        var response = new ApiResponse<object>
        {
            Status = 200,
            Message = "Create category successful",
            Data = _categoryService.GetCategories()
        };
        
        return Ok(response);
    }
}