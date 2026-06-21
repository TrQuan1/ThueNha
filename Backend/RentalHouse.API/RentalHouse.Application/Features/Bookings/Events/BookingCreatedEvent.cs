using MediatR;
using System;

namespace RentalHouse.Application.Features.Bookings.Events;

public class BookingCreatedEvent : INotification
{
    public int BookingId { get; set; }
    public int PropertyId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}