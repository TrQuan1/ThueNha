using MediatR;
using RentalHouse.Application.DTOs.Bookings;
using RentalHouse.Application.Features.Bookings.Events;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Bookings.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyCalendarRepository _calendarRepository;
    private readonly IMediator _mediator; // Đã đổi sang IMediator để Publish Event
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IPropertyCalendarRepository calendarRepository,
        IMediator mediator,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _calendarRepository = calendarRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1-6. Các bước Validate và Tính tiền giữ nguyên như cũ...
        if (request.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new Exception("Không thể đặt phòng vào ngày trong quá khứ.");
        if (request.CheckInDate >= request.CheckOutDate)
            throw new Exception("Ngày Check-out phải lớn hơn ngày Check-in.");

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà này.");

        if (property.Status != PropertyStatus.Active)
            throw new Exception("Căn nhà này hiện không mở cửa cho thuê.");

        if (property.HostId == request.TenantId)
            throw new Exception("Bạn không thể tự đặt phòng tại căn nhà của chính mình.");

        var isCalendarAvailable = await _calendarRepository.IsDateRangeAvailableAsync(
            request.PropertyId, request.CheckInDate, request.CheckOutDate);

        if (!isCalendarAvailable)
            throw new Exception("Căn nhà đã bị khóa trong khoảng thời gian này.");

        var isOverlapping = await _bookingRepository.HasOverlappingBookingAsync(
            request.PropertyId, request.CheckInDate, request.CheckOutDate);

        if (isOverlapping)
            throw new Exception("Đã có người gửi yêu cầu đặt phòng cho ngày này, vui lòng chọn ngày khác.");

        int totalDays = (request.CheckOutDate - request.CheckInDate).Days;
        decimal calculatedPrice = totalDays * property.PricePerNight;

        var booking = new Booking
        {
            TenantId = request.TenantId,
            PropertyId = request.PropertyId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            NumberOfGuests = request.NumberOfGuests,
            TotalPrice = calculatedPrice,
            Status = BookingStatus.Pending
        };

        // THỰC THI DB
        await _bookingRepository.AddAsync(booking);

        // Cần SaveChanges trước để CSDL cấp phát khóa chính (booking.Id) cho Event
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // -------------------------------------------------------------
        // 👉 PHÁT EVENT SANG MEDIATR THAY VÌ GỌI TRỰC TIẾP (PUBLISH)
        // -------------------------------------------------------------
        var bookingEvent = new BookingCreatedEvent
        {
            BookingId = booking.Id,
            PropertyId = booking.PropertyId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate
        };
        await _mediator.Publish(bookingEvent, cancellationToken);
        // -------------------------------------------------------------

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