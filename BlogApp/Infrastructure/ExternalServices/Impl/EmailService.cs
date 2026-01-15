using System.Net;
using System.Net.Mail;
using BlogApp.Infrastructure.ExternalServices.Interface;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.ExternalServices.Impl;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }
    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(to);

        using var smtp = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password
            ),
            EnableSsl = true
        };

        await smtp.SendMailAsync(message);
    }
    
}