using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> HasReviewedAsync(int bookingId)
    {
        return await _dbSet.AnyAsync(r => r.BookingId == bookingId);
    }

    public async Task<IEnumerable<Review>> GetByPropertyIdAsync(int propertyId)
    {
        return await _dbSet
            .Include(r => r.Tenant) // Kéo theo thông tin người đánh giá
            .Where(r => r.Booking.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<(double Average, int Count)> CalculatePropertyRatingAsync(int propertyId)
    {
        var reviews = await _dbSet.Where(r => r.Booking.PropertyId == propertyId).ToListAsync();
        if (!reviews.Any()) return (0, 0);

        return (Math.Round(reviews.Average(r => r.Rating), 1), reviews.Count);
    }
}