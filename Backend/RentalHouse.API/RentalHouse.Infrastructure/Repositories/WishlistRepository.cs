using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int tenantId, int propertyId)
    {
        // Kiểm tra xem căn nhà đã có trong Wishlist chưa
        return await _context.Wishlists
            .AnyAsync(w => w.TenantId == tenantId && w.PropertyId == propertyId);
    }

    public async Task<Wishlist?> GetWishlistItemAsync(int tenantId, int propertyId)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w => w.TenantId == tenantId && w.PropertyId == propertyId);
    }

    public async Task<IEnumerable<Property>> GetWishlistPropertiesByTenantAsync(int tenantId)
    {
        return await _context.Wishlists
            .Where(w => w.TenantId == tenantId)
            .Include(w => w.Property)
                .ThenInclude(p => p.Images) // Kéo theo danh sách hình ảnh để Frontend hiển thị
            .Select(w => w.Property!)
            .Where(p => !p.IsDeleted) // Chỉ kiểm tra IsDeleted trên bảng Property (Căn nhà)
            .ToListAsync();
    }

    public async Task AddAsync(Wishlist wishlist)
    {
        await _context.Wishlists.AddAsync(wishlist);
    }

    // ĐỔI TÊN HÀM TỪ Remove THÀNH Delete ĐỂ KHỚP VỚI INTERFACE
    public void Delete(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
    }
}