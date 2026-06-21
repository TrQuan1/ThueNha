using MediatR;
using RentalHouse.Application.Features.Properties.Events;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.EventHandlers;

public class NotifyAdminOnPropertyCreatedEventHandler : INotificationHandler<PropertyCreatedEvent>
{
    private readonly INotificationSender _notificationSender;
    private readonly IUserRepository _userRepository;

    public NotifyAdminOnPropertyCreatedEventHandler(
        INotificationSender notificationSender,
        IUserRepository userRepository)
    {
        _notificationSender = notificationSender;
        _userRepository = userRepository;
    }

    public async Task Handle(PropertyCreatedEvent notificationEvent, CancellationToken cancellationToken)
    {
        // 1. Lấy toàn bộ danh sách User
        var allUsers = await _userRepository.GetAllAsync();

        // 2. So sánh cực kỳ mượt mà bằng Enum gốc của bạn
        var adminIds = allUsers
            .Where(u => u.Role == Role.Admin)
            .Select(u => u.Id)
            .ToList();

        string title = "🔔 Tin đăng mới cần duyệt";
        string content = $"Có một bài đăng nhà mới: '{notificationEvent.PropertyTitle}' đang chờ bạn kiểm duyệt.";
        string redirectUrl = "/admin/properties";

        // 3. Gửi thông báo cho toàn bộ Admin
        foreach (var adminId in adminIds)
        {
            await _notificationSender.SendAsync(adminId, title, content, redirectUrl);
        }
    }
}