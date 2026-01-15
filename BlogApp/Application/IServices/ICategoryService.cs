using BlogApp.Application.DTO.Request.Category;
using BlogApp.Application.DTO.Response;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Repositories;

namespace BlogApp.Application.IServices;

public interface ICategoryService
{
    void AddCategory(CreateCategoryRequestDto category);
    List<CategoryResponseDto> GetCategories();
}