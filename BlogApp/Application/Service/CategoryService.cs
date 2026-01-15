using AutoMapper;
using BlogApp.Application.DTO.Request.Category;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IRepositories;
using BlogApp.Application.IServices;
using BlogApp.Domain.Models;
using Slugify;

namespace BlogApp.Application.Service;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }
    
    public void AddCategory(CreateCategoryRequestDto category)
    {
        // slug
        var slugHelper = new SlugHelper();
        
        var categoryToAdd = _mapper.Map<Category>(category);
        categoryToAdd.Name = category.Name.Trim().ToUpper();   
        categoryToAdd.Slug = slugHelper.GenerateSlug(category.Name);
        _categoryRepository.AddCategory(categoryToAdd);
    }

    public List<CategoryResponseDto> GetCategories()
    {
        var categories = _categoryRepository.GetCategories();
        return _mapper.Map<List<CategoryResponseDto>>(_categoryRepository.GetCategories());
    }


}