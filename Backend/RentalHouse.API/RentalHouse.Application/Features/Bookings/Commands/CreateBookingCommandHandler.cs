using MediatR;
using RentalHouse.Application.DTOs.Bookings;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Bookings.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyCalendarRepository _calendarRepository;
    private readonly INotificationRepository _notificationRepository; // Đã thêm repo notification
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IPropertyCalendarRepository calendarRepository,
        INotificationRepository notificationRepository, // Tiêm vào constructor
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _calendarRepository = calendarRepository;
        _notificationRepository = notificationRepository; // Gán
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate: Không đặt phòng trong quá khứ và logic Check-in/Check-out
        if (request.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new Exception("Không thể đặt phòng vào ngày trong quá khứ.");
        if (request.CheckInDate >= request.CheckOutDate)
            throw new Exception("Ngày Check-out phải lớn hơn ngày Check-in.");

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà này.");

        // 2. Validate: Nhà phải đang mở cửa cho thuê
        if (property.Status != PropertyStatus.Active)
            throw new Exception("Căn nhà này hiện không mở cửa cho thuê.");

        // 3. Validate: Chống tự đặt nhà của mình
        if (property.HostId == request.TenantId)
            throw new Exception("Bạn không thể tự đặt phòng tại căn nhà của chính mình.");

        // 4. Validate lớp 1: KIỂM TRA LỊCH CỦA HOST (PropertyCalendar)
        var isCalendarAvailable = await _calendarRepository.IsDateRangeAvailableAsync(
            request.PropertyId,
            request.CheckInDate,
            request.CheckOutDate);

        if (!isCalendarAvailable)
            throw new Exception("Căn nhà đã bị khóa trong khoảng thời gian này.");

        // 5. Validate lớp 2: KIỂM TRA CÓ AI ĐANG "XÍ CHỖ" CHƯA (Bảng Booking)
        var isOverlapping = await _bookingRepository.HasOverlappingBookingAsync(
            request.PropertyId,
            request.CheckInDate,
            request.CheckOutDate);

        if (isOverlapping)
            throw new Exception("Đã có người gửi yêu cầu đặt phòng cho ngày này, vui lòng chọn ngày khác.");

        // 6. TÍNH TOÁN GIÁ TIỀN AN TOÀN TRÊN BACKEND (Không tin tưởng Frontend)
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

        await _bookingRepository.AddAsync(booking);

        // -------------------------------------------------------------
        // 👉 GỬI THÔNG BÁO CHO CHỦ NHÀ
        // -------------------------------------------------------------
        var notification = new Notification
        {
            UserId = property.HostId, // Gửi cho Chủ nhà (Host)
            Title = "🎉 Có yêu cầu đặt phòng mới!",
            Content = $"Căn nhà '{property.Title}' của bạn vừa được đặt từ ngày {request.CheckInDate:dd/MM/yyyy} đến {request.CheckOutDate:dd/MM/yyyy}. Hãy vào xem ngay!",
            RedirectUrl = "/host/bookings", // Đường link để chủ nhà nhảy ra trang quản lý yêu cầu
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        await _notificationRepository.AddAsync(notification);
        // -------------------------------------------------------------

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