using BlogApp.Domain.Enums;

namespace BlogApp.Application.DTO.Response;

public record BlogResponseDto(
    int Id,
    string Title,
    string Slug,
    string Content,
    UserProfileResponseDto Author,
    BlogStatus Status,
    string? Avatar,
    DateTime? PublishedAt,
    DateTime UpdatedAt,
    List<CategoryResponseDto> Categories,
    List<TagReponseDto> Tags,
    List<CommentResponseDto> Comments
    );