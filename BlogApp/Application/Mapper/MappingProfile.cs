using BlogApp.Application.DTO.Request;
using BlogApp.Application.DTO.Request.Authenticate;
using BlogApp.Application.DTO.Request.Category;
using BlogApp.Application.DTO.Request.Comment;
using BlogApp.Application.DTO.Response;
using BlogApp.Domain.Models;

namespace BlogApp.Application.Mapper;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<RegisterRequestDto, User>().ForSourceMember(src => src.ConfirmPassword, 
            opt => opt.DoNotValidate());
        CreateMap<UserProfileRequestDto, User>();
        CreateMap<User, UserProfileResponseDto>();
        
        // Category
        CreateMap<CreateCategoryRequestDto, Category>();
        CreateMap<Category, CategoryResponseDto>();
        
        // Tag
        CreateMap<Tag, TagReponseDto>();
        
        // Blog
        CreateMap<Blog, BlogResponseDto>()
            .ForCtorParam("Author",
                opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("Categories",
                opt => opt.MapFrom(src =>
                    src.BlogCategories.Select(bc => bc.Category).ToList()))
            .ForCtorParam("Tags",
                opt => opt.MapFrom(src =>
                    src.BlogTags.Select(bt => bt.Tag).ToList()));

        // Comment
        CreateMap<CommentRequestDto, Comment>();
        CreateMap<Comment, CommentResponseDto>()
            .ForCtorParam("UserName",
                opt => opt.MapFrom(src => src.User.UserName))
            .ForCtorParam("BlogId",
                opt => opt.MapFrom(src => src.BlogId));
    }
}