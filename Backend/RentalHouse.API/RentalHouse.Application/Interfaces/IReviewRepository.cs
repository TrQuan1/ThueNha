using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<bool> HasReviewedAsync(int bookingId);
    Task<IEnumerable<Review>> GetByPropertyIdAsync(int propertyId);
    Task<(double Average, int Count)> CalculatePropertyRatingAsync(int propertyId);
}