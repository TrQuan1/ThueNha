using MediatR;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Bookings.Events;

public class BookingStatusChangedEvent : INotification
{
    public int BookingId { get; set; }
    public int PropertyId { get; set; }
    public int HostId { get; set; }
    public int TenantId { get; set; }
    public int TriggeredByUserId { get; set; }
    public BookingStatus TargetStatus { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
}