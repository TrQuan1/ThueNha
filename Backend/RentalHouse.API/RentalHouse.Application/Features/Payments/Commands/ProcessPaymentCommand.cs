using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Payments.Commands;

public class ProcessPaymentCommand : IRequest<bool>
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "VNPay";
}

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentCommandHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Không tìm thấy đơn đặt phòng.");

        // Giả lập xử lý thanh toán thành công
        var payment = new Payment
        {
            BookingId = request.BookingId,
            Amount = request.Amount,
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Success
        };

        await _paymentRepository.AddAsync(payment);

        // Cập nhật trạng thái Booking
        booking.Status = BookingStatus.Approved;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}