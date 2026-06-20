using MediatR;
using RentalHouse.Application.Features.Properties.Events;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.EventHandlers;

public class NotifyHostOnPropertyStatusChangedEventHandler : INotificationHandler<PropertyStatusChangedEvent>
{
    private readonly INotificationSender _notificationSender;

    public NotifyHostOnPropertyStatusChangedEventHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task Handle(PropertyStatusChangedEvent notificationEvent, CancellationToken cancellationToken)
    {
        string title;
        string content;
        string redirectUrl = "/host/properties";

        if (notificationEvent.TargetStatus == PropertyStatus.Active)
        {
            title = "🎉 Tin đăng đã được duyệt";
            content = $"Chúc mừng! Căn nhà '{notificationEvent.PropertyTitle}' của bạn đã được kiểm duyệt và hiển thị trên hệ thống.";
        }
        else if (notificationEvent.TargetStatus == PropertyStatus.Rejected)
        {
            title = "❌ Tin đăng bị từ chối";
            content = $"Bài đăng '{notificationEvent.PropertyTitle}' của bạn đã bị quản trị viên từ chối. Vui lòng kiểm tra lại nội dung.";
        }
        else
        {
            return; // Các trạng thái khác bỏ qua
        }

        await _notificationSender.SendAsync(notificationEvent.HostId, title, content, redirectUrl);
    }
}