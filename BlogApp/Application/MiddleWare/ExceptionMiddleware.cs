using BlogApp.Application.DTO.Response;

namespace BlogApp.Application.MiddleWare;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex) // Bắt AppException trước
        {
            _logger.LogError(ex, "AppException caught");
            await HandleAppExceptionAsync(context, ex);
        }
        catch (Exception ex) // Bắt tất cả exception khác
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleGenericExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleAppExceptionAsync(HttpContext context, AppException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest; // giống ResponseEntity.badRequest()

        var response = new ApiResponse<object>
        {
            Status = ex.Code,
            Message = ex.Message,
            Data = null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
    
    private static Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ApiResponse<object>
        {
            Status = 500,
            Message = ex.Message,
            Data = null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}