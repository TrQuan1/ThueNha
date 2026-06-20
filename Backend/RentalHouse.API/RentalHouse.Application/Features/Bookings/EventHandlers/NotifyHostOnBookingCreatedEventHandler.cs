using MediatR;
using RentalHouse.Application.Features.Bookings.Events;
using RentalHouse.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Bookings.EventHandlers;

public class NotifyHostOnBookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly INotificationSender _notificationSender;
    private readonly IPropertyRepository _propertyRepository;

    public NotifyHostOnBookingCreatedEventHandler(
        INotificationSender notificationSender,
        IPropertyRepository propertyRepository)
    {
        _notificationSender = notificationSender;
        _propertyRepository = propertyRepository;
    }

    public async Task Handle(BookingCreatedEvent notificationEvent, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin căn nhà để biết HostId là ai
        var property = await _propertyRepository.GetByIdAsync(notificationEvent.PropertyId);
        if (property == null) return;

        // 2. Chuẩn bị nội dung
        var title = "🎉 Có yêu cầu đặt phòng mới!";
        var content = $"Căn nhà '{property.Title}' của bạn vừa được đặt từ ngày {notificationEvent.CheckInDate:dd/MM/yyyy} đến {notificationEvent.CheckOutDate:dd/MM/yyyy}. Hãy vào xem ngay!";
        var redirectUrl = "/host/bookings";

        // 3. Gửi thông báo
        await _notificationSender.SendAsync(property.HostId, title, content, redirectUrl);
    }
}