namespace BlogApp.Infrastructure.ExternalServices.Interface;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}