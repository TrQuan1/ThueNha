using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetBookedDatesQueryHandler : IRequestHandler<GetBookedDatesQuery, List<string>>
{
    private readonly IPropertyCalendarRepository _calendarRepository;
    private readonly IBookingRepository _bookingRepository; // Tiêm thêm Kho Booking

    public GetBookedDatesQueryHandler(
        IPropertyCalendarRepository calendarRepository,
        IBookingRepository bookingRepository)
    {
        _calendarRepository = calendarRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<List<string>> Handle(GetBookedDatesQuery request, CancellationToken cancellationToken)
    {
        // Dùng HashSet để tự động loại bỏ các ngày bị trùng lặp
        var bookedDates = new HashSet<string>();

        // 1. Lấy các ngày khóa từ PropertyCalendar (Do Host tự khóa hoặc đã Approve)
        var calendarDates = await _calendarRepository.GetBookedDatesAsync(request.PropertyId);
        foreach (var date in calendarDates)
        {
            bookedDates.Add(date);
        }

        // 2. Lấy thêm các ngày từ các Booking đang "Xí chỗ" (Pending/Approved)
        var activeBookings = await _bookingRepository.GetActiveBookingsForPropertyAsync(request.PropertyId);
        foreach (var booking in activeBookings)
        {
            // Chạy vòng lặp từ ngày CheckIn đến sát ngày CheckOut
            for (var d = booking.CheckInDate.Date; d < booking.CheckOutDate.Date; d = d.AddDays(1))
            {
                bookedDates.Add(d.ToString("yyyy-MM-dd"));
            }
        }

        return bookedDates.ToList();
    }
}