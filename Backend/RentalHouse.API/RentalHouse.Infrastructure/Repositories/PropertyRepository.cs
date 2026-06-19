using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Property?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Images)
            .Include(p => p.PropertyFacilities).ThenInclude(pf => pf.Facility)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public override async Task<IEnumerable<Property>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Images)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetPropertiesByHostAsync(int hostId)
    {
        return await _dbSet
            .Include(p => p.Images) // Kéo theo cả hình ảnh để UI hiển thị
            .Where(p => p.HostId == hostId && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Property> Items, int TotalCount)> SearchAsync(
        string? keyword, string? location, decimal? minPrice, decimal? maxPrice,
        int? minGuests, double? minRating, int pageNumber, int pageSize, string? sortBy)
    {
        // 1. Khởi tạo Query (Chưa gọi xuống DB)
        var query = _dbSet.Where(p => p.Status == Domain.Enums.PropertyStatus.Active).AsQueryable();

        // 2. Áp dụng các bộ lọc (Dynamic LINQ)
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.Title.Contains(keyword) || p.Description.Contains(keyword));

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(p => p.Address.Contains(location));

        if (minPrice.HasValue)
            query = query.Where(p => p.PricePerNight >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.PricePerNight <= maxPrice.Value);

        if (minGuests.HasValue)
            query = query.Where(p => p.MaxGuests >= minGuests.Value);

        if (minRating.HasValue)
            query = query.Where(p => p.AverageRating >= minRating.Value);

        // 3. Đếm tổng số lượng bản ghi thỏa mãn điều kiện (Để làm phân trang)
        var totalCount = await query.CountAsync();

        // 4. Áp dụng sắp xếp (Sorting)
        query = sortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(p => p.PricePerNight),
            "price_desc" => query.OrderByDescending(p => p.PricePerNight),
            "rating_desc" => query.OrderByDescending(p => p.AverageRating),
            _ => query.OrderByDescending(p => p.CreatedAt) // Mặc định: Mới nhất lên đầu
        };

        // 5. Phân trang (Skip & Take) và thực thi Query xuống Database
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<int> CountPropertiesByHostIdAsync(int hostId)
    {
        return await _dbSet.CountAsync(p => p.HostId == hostId);
    }
}