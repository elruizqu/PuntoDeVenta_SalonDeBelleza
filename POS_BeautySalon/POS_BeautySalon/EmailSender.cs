using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"]),
            Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:From"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            // Log the exception or handle it accordingly
            throw new InvalidOperationException("Failure sending mail.", ex);
        }
    }
}
