using MediatR;
using RentalHouse.Application.Features.Bookings.Events;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Bookings.EventHandlers;

public class NotifyOnBookingStatusChangedEventHandler : INotificationHandler<BookingStatusChangedEvent>
{
    private readonly INotificationSender _notificationSender;

    public NotifyOnBookingStatusChangedEventHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task Handle(BookingStatusChangedEvent notificationEvent, CancellationToken cancellationToken)
    {
        string title;
        string content;
        string redirectUrl;
        int targetUserId;

        // Trường hợp 1: Khách hàng (Tenant) chủ động Hủy phòng
        if (notificationEvent.TriggeredByUserId == notificationEvent.TenantId &&
            notificationEvent.TargetStatus == BookingStatus.Cancelled)
        {
            targetUserId = notificationEvent.HostId;
            title = "⚠️ Khách đã hủy đặt phòng";
            content = $"Khách hàng đã hủy yêu cầu đặt phòng tại căn nhà '{notificationEvent.PropertyTitle}'.";
            redirectUrl = "/host/bookings";
        }
        // Trường hợp 2: Chủ nhà (Host) thao tác Duyệt / Từ chối / Hủy
        else if (notificationEvent.TriggeredByUserId == notificationEvent.HostId)
        {
            targetUserId = notificationEvent.TenantId;
            redirectUrl = "/my-bookings";

            switch (notificationEvent.TargetStatus)
            {
                case BookingStatus.Approved:
                    title = "✅ Đặt phòng thành công";
                    content = $"Yêu cầu đặt phòng tại '{notificationEvent.PropertyTitle}' của bạn đã được chủ nhà chấp nhận.";
                    break;
                case BookingStatus.Rejected:
                    title = "❌ Đặt phòng bị từ chối";
                    content = $"Rất tiếc, chủ nhà đã từ chối yêu cầu đặt phòng của bạn tại '{notificationEvent.PropertyTitle}'.";
                    break;
                case BookingStatus.Cancelled:
                    title = "⚠️ Đặt phòng bị hủy";
                    content = $"Chủ nhà đã hủy lịch đặt phòng của bạn tại '{notificationEvent.PropertyTitle}'.";
                    break;
                default:
                    return; // Bỏ qua các trạng thái không cần thông báo
            }
        }
        else
        {
            return; // Nếu do hệ thống auto cancel thì có thể bổ sung thêm logic ở đây
        }

        await _notificationSender.SendAsync(targetUserId, title, content, redirectUrl);
    }
}