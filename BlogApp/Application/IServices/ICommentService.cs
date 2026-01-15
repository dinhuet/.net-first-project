using BlogApp.Application.DTO.Request.Comment;
using BlogApp.Application.DTO.Response;
using BlogApp.Domain.Models;

namespace BlogApp.Application.IServices;

public interface ICommentService
{
    Task<CommentResponseDto> CreateComment(CommentRequestDto dto, string email);
}