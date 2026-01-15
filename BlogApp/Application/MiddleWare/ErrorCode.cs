namespace BlogApp.Application.MiddleWare;

public class ErrorCode
{
    /*public static readonly ErrorCode ProductNotFound = new ErrorCode(404, "Product not found");
    public static readonly ErrorCode InvalidRequest = new ErrorCode(400, "Invalid request");*/
    public static readonly ErrorCode UserNotFound = new ErrorCode(404, "User Not Found");
    public static readonly ErrorCode ImageNotAllowed = new ErrorCode(404, "File is not allowed");
    public static readonly ErrorCode ImageTooLarge = new ErrorCode(404, "File must be smaller 5Mb");
    public static readonly ErrorCode ContentTypeImageNotAllowed = new ErrorCode(404, "Content-Type is not allowed");
    /*public static readonly ErrorCode UserExist = new ErrorCode(404, "Email is already registered");*/
    
    public static readonly ErrorCode NotAuthenticate = new ErrorCode(404, "User is not Authenticate");
    public static readonly ErrorCode PasswordIsNotMatch = new ErrorCode(404, "Password is incorrect");
    public static readonly ErrorCode ConfirmPasswordIsNotMatch = new ErrorCode(404, "Confirm password is not match");
    public static readonly ErrorCode FileIsEmpty = new ErrorCode(404, "File is empty");
    public static readonly ErrorCode OtpIsNotTrue = new ErrorCode(404, "Otp is not true");
    public static readonly ErrorCode OtpIsInvalid = new ErrorCode(404, "Otp is used or expired, you must get other");
    public static readonly ErrorCode OldPasswordIsNotTrue = new ErrorCode(404, "Old password is not true");
    public static readonly ErrorCode CategoryNotExist = new ErrorCode(404, "Category is  not exist");
    public static readonly ErrorCode BlogIsNotExist = new ErrorCode(404, "Blog is not exist");
    public static readonly ErrorCode ParentCommentIsNotMatch = new ErrorCode(404, "BlogId is not match with parent's BlogID");

    
    public int Code { get; }
    public string Message { get; }

    private ErrorCode(int code, string message)
    {
        Code = code;
        Message = message;
    }
}