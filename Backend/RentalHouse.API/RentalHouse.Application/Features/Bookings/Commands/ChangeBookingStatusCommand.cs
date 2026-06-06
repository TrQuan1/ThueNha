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
    private readonly IPropertyRepository _propertyRepository; // THÊM REPOSITORY NÀY
    private readonly IUnitOfWork _unitOfWork;

    // Cập nhật Constructor để tiêm IPropertyRepository
    public ChangeBookingStatusCommandHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangeBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Không tìm thấy thông tin đặt phòng.");

        // Lấy property chuẩn Clean Architecture thông qua Repository
        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy thông tin căn nhà.");

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
                if (booking.TenantId != request.UserId)
                    throw new UnauthorizedAccessException("Bạn không có quyền hủy lịch đặt phòng của người khác.");
                if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Rejected)
                    throw new Exception("Không thể hủy lịch đặt phòng này.");
                break;
        }

        booking.Status = request.TargetStatus;
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}