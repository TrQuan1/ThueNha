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
        // 👉 Đã sửa các khóa để khớp với EmailSettings trong appsettings.json
        var host = _config["EmailSettings:Host"];
        var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
        var email = _config["EmailSettings:Email"];
        var password = _config["EmailSettings:Password"]; // Đã đổi AppPassword -> Password

        // Kiểm tra xem email có bị null không để báo lỗi rõ ràng
        if (string.IsNullOrEmpty(email))
        {
            throw new Exception("Không tìm thấy cấu hình Email trong appsettings.json");
        }

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(email, password),
            EnableSsl = true,
            UseDefaultCredentials = false // Quan trọng: Phải set false cho Gmail
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(email, "RentalHouse Admin"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }
}