using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    // Hàm truy vấn đặc thù: Lấy danh sách nhà do một Host cụ thể đăng
    Task<IEnumerable<Property>> GetPropertiesByHostAsync(int hostId);
    Task<(IEnumerable<Property> Items, int TotalCount)> SearchAsync(
        string? keyword,
        string? location,
        decimal? minPrice,
        decimal? maxPrice,
        int? minGuests,
        double? minRating,
        int pageNumber,
        int pageSize,
        string? sortBy);
}