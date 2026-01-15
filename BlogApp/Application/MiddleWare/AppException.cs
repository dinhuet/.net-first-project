
namespace BlogApp.Application.MiddleWare;
public class AppException : Exception
{
    public int Code { get; }
    
    public AppException(ErrorCode errorCode) : base(errorCode.Message)
    {
        Code = errorCode.Code;
    }
}