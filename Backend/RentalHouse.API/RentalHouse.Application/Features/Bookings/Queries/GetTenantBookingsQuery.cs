using MediatR;
using RentalHouse.Application.DTOs.Bookings;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Bookings.Queries;

public class GetTenantBookingsQuery : IRequest<IEnumerable<BookingDto>>
{
    public int TenantId { get; set; }
}

public class GetTenantBookingsQueryHandler : IRequestHandler<GetTenantBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetTenantBookingsQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetTenantBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsByTenantAsync(request.TenantId);

        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            TenantId = b.TenantId,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            // 👉 THÊM DÒNG NÀY ĐỂ LẤY TÊN NHÀ (Nhớ thêm thuộc tính PropertyTitle vào file BookingDto.cs nhé)
            PropertyTitle = b.Property?.Title ?? $"Căn nhà #{b.PropertyId}"
        });
    }
}