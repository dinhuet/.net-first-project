using AutoMapper;
using BlogApp.Application.DTO.Request.Comment;
using BlogApp.Application.DTO.Response;
using BlogApp.Application.IRepositories;
using BlogApp.Application.IServices;
using BlogApp.Application.MiddleWare;
using BlogApp.Domain.Models;

namespace BlogApp.Application.Service;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IMapper mapper, IUserRepository userRepository, 
        IUnitOfWork uow)
    {
        _uow = uow;
        _userRepository = userRepository;
        _mapper = mapper;
        _commentRepository = commentRepository;
    }
    
    public async Task<CommentResponseDto> CreateComment(CommentRequestDto dto, string email)
    {
        var userId = await _userRepository.GetUserIdByEmailAsync(email);
        
        if (dto.ParentId != null)
        {
            Console.WriteLine("------------------as----parentId: " +  dto.ParentId.Value);
            //var parentComment = await _commentRepository.GetByIdAsync(dto.ParentId.Value);

            /*if (parentComment.BlogId != dto.BlogId)
                throw new AppException(ErrorCode.ParentCommentIsNotMatch);
            parentComment.TotalReplies++;*/
            
            // await _commentRepository.Update(parentComment);*/
        }
        
        var comment = _mapper.Map<Comment>(dto);
        comment.UserId = userId;
        Console.WriteLine("----------------------parentId: " +  dto.ParentId);
        
        
        var response = await _commentRepository.AddCommentAsync(comment);

        await _uow.SaveChangesAsync();
        return _mapper.Map<CommentResponseDto>(response);
    }

}