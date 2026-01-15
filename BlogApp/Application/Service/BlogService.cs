using AutoMapper;
using BlogApp.Application.DTO.Page;
using BlogApp.Application.DTO.Request.Blog;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.Event;
using BlogApp.Application.Index;
using BlogApp.Application.IRepositories;
using BlogApp.Application.IServices;
using BlogApp.Application.MiddleWare;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.ExternalServices;
using BlogApp.Infrastructure.ExternalServices.Impl;
using BlogApp.Infrastructure.ExternalServices.Interface;
using BlogApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slugify;

namespace BlogApp.Application.Service;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUploadService _uploadService;
    private readonly IMapper _mapper;
    private readonly IBlogSuggestService _blogSuggestService;
    private readonly KafkaProducer _kafkaProducer;

    public BlogService(IBlogRepository blogRepository,  ICategoryRepository categoryRepository,  
        ITagRepository tagRepository, IUserRepository userRepository, IUploadService uploadService,  IMapper mapper
        , IBlogSuggestService blogSuggestService, KafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
        _blogSuggestService = blogSuggestService;
        _mapper = mapper;
        _uploadService = uploadService;
        _userRepository = userRepository;
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _blogRepository = blogRepository;
    }
    
    bool ExistCategoryById(int categoryId)
    {
        return (_categoryRepository.GetCategoryById(categoryId) != null);
    }
    
    public async Task CreateAsync(CreateBlogRequestDto request, string email)
    {
                    
        // thu vien slug
        var slugHelper = new SlugHelper();
        
        User user = _userRepository.GetUserByEmail(email);
        if (user == null) throw new AppException(ErrorCode.UserNotFound);
        
        var blog = new Blog
        {
            Title = request.Title,
            Content = request.Content,
            Slug = slugHelper.GenerateSlug(request.Title),
            PublishedAt = request.Status == BlogStatus.Published ?  DateTime.Now : null,
            AuthorId = user.Id,
            Status =  request.Status,
            Avatar = request.Avatar != null
                ? await _uploadService.UploadImageAsync(request.Avatar)
                : string.Empty,

        };

        if (request.Categories?.Any() == true )
        {
            foreach (var categoryId in request.Categories)
            {
                if (!ExistCategoryById(categoryId)) throw new AppException(ErrorCode.CategoryNotExist);
            }
            
            blog.BlogCategories = request.Categories
                .Distinct()
                .Select(categoryId => new BlogCategory
                {
                    CategoryId = categoryId
                })
                .ToList();
        }
        
        // xu ly tag
        if (request.TagNames?.Any() == true)
        {
            var normalizedTagNames = request.TagNames
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .ToList();
            
            // tag da ton tai
            var existingTags  = _tagRepository.GetExistingTask(normalizedTagNames);
            
            // tag chua co
            var existingTagNames = existingTags
                .Select(t => t.Name.ToLower())
                .ToHashSet();

            var newTags = normalizedTagNames
                .Where(name => !existingTagNames.Contains(name))
                .Select(name => new Tag
                {
                    Name = name,
                    Slug = slugHelper.GenerateSlug(name),
                })
                .ToList();
            
            if (newTags.Any())
            {
                _tagRepository.AddMultipleTag(newTags);
                existingTags.AddRange(newTags);
            }
            
            blog.BlogTags = existingTags
                .Select(tag => new BlogTag
                {
                    TagId = tag.Id
                })
                .ToList();
        }
        
        await _blogRepository.AddAsync(blog);
        
        await _kafkaProducer.PublishAsync(
            "blog-events",
            key: blog.Id.ToString(),
            value: new BlogCreatedEvent
            {
                BlogId = blog.Id,
                AuthorId = blog.AuthorId,
                Status = blog.Status,
            }
        );
    }

    public PagedResult<BlogResponseDto> GetPageBlog(BlogQueryDto dto)
    {
        var blogs = _blogRepository.GetPageBlog()
            .Where(b => b.Status == BlogStatus.Published)
            .OrderByDescending(b => b.PublishedAt); 
        
        var total = blogs.Count();
        
        var items = blogs.Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToList();
        
        return new PagedResult<BlogResponseDto>
        {
            Items = _mapper.Map<List<BlogResponseDto>>(items),
            TotalItems = total,
            Page = dto.Page,
            PageSize = dto.PageSize
        };
    }

    public PagedResult<BlogResponseDto> GetMyBlog(string email, BlogQueryDto dto)

    {
        var blogs = _blogRepository.GetPageBlog()
            .Where(b => b.Status == BlogStatus.Published && b.Author.Email == email)
            .OrderByDescending(b => b.PublishedAt); 
        
        var total = blogs.Count();
        
        var items = blogs.Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToList();
        
        return new PagedResult<BlogResponseDto>
        {
            Items = _mapper.Map<List<BlogResponseDto>>(items),
            TotalItems = total,
            Page = dto.Page,
            PageSize = dto.PageSize
        };
    }

    public async Task PublishBlog(int id)
    {
        await _blogRepository.PublishBlog(id);
        
    }
    
    // elastic
    public async Task<List<BlogIndex>> SearchBlogsAsync(string query)
    {
        var results = await _blogSuggestService.SearchAsync(query);
        return results;
    }
}