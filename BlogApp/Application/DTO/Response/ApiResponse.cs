namespace BlogApp.Application.DTO.Response;

public class ApiResponse<T>
{
    public int Status { get; set; } = 200;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

}