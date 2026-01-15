namespace BlogApp.Application.DTO.Request.Comment;

public record CommentRequestDto
(
    int BlogId,
    int? ParentId,
    string Content
    );