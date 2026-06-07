using RentalHouse.Domain.Entities;
namespace RentalHouse.Application.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByBookingIdAsync(int bookingId);
}