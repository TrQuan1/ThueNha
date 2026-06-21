using Microsoft.Extensions.Configuration;
using RentalHouse.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RentalHouse.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var host = _config["SmtpSettings:Host"];
        var port = int.Parse(_config["SmtpSettings:Port"] ?? "587");
        var email = _config["SmtpSettings:Email"];
        var password = _config["SmtpSettings:AppPassword"]; // Mật khẩu ứng dụng Gmail

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(email, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(email!, "RentalHouse Admin"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }
}