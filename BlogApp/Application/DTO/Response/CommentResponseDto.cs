namespace BlogApp.Application.DTO.Response;

public record CommentResponseDto(
    int BlogId,
    string UserName,
    string Content,
    DateTime CreatedAt,
    CommentResponseDto? Parent,
    ICollection<CommentResponseDto> Replies
    );