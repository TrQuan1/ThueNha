using System;
using System.Threading.Tasks;
using RentalHouse.Application.Interfaces;
// Cố tình KHÔNG using RentalHouse.Domain.Entities ở cục bộ để dùng namespace tuyệt đối phía dưới

namespace RentalHouse.Application.Services;

public class NotificationSender : INotificationSender
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationSender(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SendAsync(int userId, string title, string content, string redirectUrl)
    {
        // Khởi tạo Entity Notification với đường dẫn Namespace tuyệt đối 
        // để tránh xung đột với interface INotification của thư viện MediatR
        var notification = new RentalHouse.Domain.Entities.Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            RedirectUrl = redirectUrl,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync(default);
    }
}