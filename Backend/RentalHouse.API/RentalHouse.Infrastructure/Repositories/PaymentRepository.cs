using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Payment?> GetByBookingIdAsync(int bookingId)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
    }
}