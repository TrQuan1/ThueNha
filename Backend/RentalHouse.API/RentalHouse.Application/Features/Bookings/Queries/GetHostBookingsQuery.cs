using MediatR;
using RentalHouse.Application.DTOs.Bookings;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Bookings.Queries;

public class GetHostBookingsQuery : IRequest<IEnumerable<BookingDto>>
{
    public int HostId { get; set; }
}

public class GetHostBookingsQueryHandler : IRequestHandler<GetHostBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetHostBookingsQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetHostBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsByHostAsync(request.HostId);

        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            TenantId = b.TenantId,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Status = b.Status
        });
    }
}