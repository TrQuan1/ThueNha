using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Bookings.Commands;

// Command này không cần nhận tham số gì từ bên ngoài
public class UpdateCompletedBookingsCommand : IRequest<bool> { }

public class UpdateCompletedBookingsCommandHandler : IRequestHandler<UpdateCompletedBookingsCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompletedBookingsCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCompletedBookingsCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy tất cả các chuyến đi
        var allBookings = await _bookingRepository.GetAllAsync();

        // 2. Lấy giờ hiện tại (đã tính phút/giây) chứ không lấy .Date (0h sáng) nữa
        var now = DateTime.UtcNow.AddHours(7);

        // 3. Lọc ra những đơn đang "Approved" (Đã xác nhận) mà ngày CheckOut đã trôi qua
        var expiredBookings = allBookings.Where(b =>
            b.Status == BookingStatus.Approved &&
            // Nếu Thời gian hiện tại >= 12h00 trưa của ngày trả phòng là cho Done luôn!
            now >= b.CheckOutDate.Date.AddHours(12)
        ).ToList();

        // Nếu không có đơn nào quá hạn thì kết thúc luôn
        if (!expiredBookings.Any()) return true;

        // 4. Nếu có, đổi trạng thái sang Completed (Hoàn thành)
        foreach (var booking in expiredBookings)
        {
            booking.Status = BookingStatus.Completed;
            _bookingRepository.Update(booking);
        }

        // 5. Lưu xuống Database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}