using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Backend.API.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailMessage = new MimeMessage();
        var fromEmail = _config["EmailSettings:SenderEmail"] ?? "test@example.com";
        var password = _config["EmailSettings:SenderPassword"] ?? "";
        
        emailMessage.From.Add(new MailboxAddress("MedEquip", fromEmail));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromEmail, password);
            await client.SendAsync(emailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
