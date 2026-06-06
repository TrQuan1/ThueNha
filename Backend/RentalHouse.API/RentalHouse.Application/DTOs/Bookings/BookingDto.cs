using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.DTOs.Bookings;

public class BookingDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int TenantId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
}