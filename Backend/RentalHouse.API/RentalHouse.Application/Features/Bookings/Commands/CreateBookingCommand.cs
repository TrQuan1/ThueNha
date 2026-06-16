using MediatR;
using RentalHouse.Application.DTOs.Bookings;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Bookings.Commands;

public class CreateBookingCommand : IRequest<BookingDto>
{
    public int TenantId { get; set; }
    public int PropertyId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(IBookingRepository bookingRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate: Quá khứ & Logic ngày
        if (request.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new Exception("Không thể đặt phòng vào ngày trong quá khứ.");
        if (request.CheckInDate >= request.CheckOutDate)
            throw new Exception("Ngày Check-out phải lớn hơn ngày Check-in.");

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà này.");    

        // 2. Validate: Trạng thái nhà
        if (property.Status != PropertyStatus.Active)
            throw new Exception("Căn nhà này hiện không mở cửa cho thuê.");

        // 3. Validate: Không tự đặt nhà của mình
        if (property.HostId == request.TenantId)
            throw new Exception("Bạn không thể tự đặt phòng tại căn nhà của chính mình.");

        // 4. Validate: Trùng lịch
        var isOverlapping = await _bookingRepository.HasOverlappingBookingAsync(request.PropertyId, request.CheckInDate, request.CheckOutDate);
        if (isOverlapping)
            throw new Exception("Căn nhà đã có người đặt trong khoảng thời gian này.");

        // 5. Tính toán giá
        int totalDays = (request.CheckOutDate - request.CheckInDate).Days;
        decimal calculatedPrice = totalDays * property.PricePerNight;

        var booking = new Booking
        {
            PropertyId = request.PropertyId,
            TenantId = request.TenantId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalPrice = calculatedPrice,
            Status = BookingStatus.Pending // Chờ Host duyệt
        };

        await _bookingRepository.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BookingDto
        {
            Id = booking.Id,
            PropertyId = booking.PropertyId,
            TenantId = booking.TenantId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status
        };
    }
}