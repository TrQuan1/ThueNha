using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Bookings.Commands;

public class ChangeBookingStatusCommand : IRequest<bool>
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public BookingStatus TargetStatus { get; set; }
}

public class ChangeBookingStatusCommandHandler : IRequestHandler<ChangeBookingStatusCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyCalendarRepository _calendarRepository; // Đã thêm Kho Lịch
    private readonly IUnitOfWork _unitOfWork;

    public ChangeBookingStatusCommandHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IPropertyCalendarRepository calendarRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _calendarRepository = calendarRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangeBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Không tìm thấy thông tin đặt phòng.");

        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy thông tin căn nhà.");

        // Ghi nhớ trạng thái cũ trước khi đổi
        var previousStatus = booking.Status;

        // 1. KIỂM TRA QUYỀN VÀ TÍNH HỢP LỆ
        switch (request.TargetStatus)
        {
            case BookingStatus.Approved:
            case BookingStatus.Rejected:
                if (property.HostId != request.UserId)
                    throw new UnauthorizedAccessException("Chỉ Chủ nhà mới có quyền duyệt/từ chối yêu cầu này.");
                if (booking.Status != BookingStatus.Pending)
                    throw new Exception("Chỉ có thể duyệt/từ chối các yêu cầu đang chờ xử lý.");
                break;

            case BookingStatus.Cancelled:
                // Cả Host và Tenant đều có thể hủy chuyến đi (nếu cần thiết)
                if (booking.TenantId != request.UserId && property.HostId != request.UserId)
                    throw new UnauthorizedAccessException("Bạn không có quyền hủy lịch đặt phòng này.");
                if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Rejected)
                    throw new Exception("Không thể hủy lịch đặt phòng này.");
                break;
        }

        // 2. CẬP NHẬT TRẠNG THÁI BOOKING
        booking.Status = request.TargetStatus;
        _bookingRepository.Update(booking);

        // 3. ĐỒNG BỘ LỊCH TRỐNG (CALENDAR MAGIC)
        if (request.TargetStatus == BookingStatus.Approved)
        {
            // Bấm Duyệt -> Khóa lịch
            await _calendarRepository.MarkDatesAsBookedAsync(booking.PropertyId, booking.CheckInDate, booking.CheckOutDate);
        }
        else if (request.TargetStatus == BookingStatus.Cancelled && previousStatus == BookingStatus.Approved)
        {
            // Đã duyệt rồi mà giờ lại Hủy -> Bắt buộc phải NHẢ LỊCH
            await _calendarRepository.ReleaseDatesAsync(booking.PropertyId, booking.CheckInDate, booking.CheckOutDate);
        }

        // 4. LƯU DATABASE
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}