using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

// KHÔNG kế thừa GenericRepository<Wishlist> nữa
public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int tenantId, int propertyId)
    {
        return await _context.Wishlists.AnyAsync(w => w.TenantId == tenantId && w.PropertyId == propertyId);
    }

    public async Task<Wishlist?> GetWishlistItemAsync(int tenantId, int propertyId)
    {
        return await _context.Wishlists.FirstOrDefaultAsync(w => w.TenantId == tenantId && w.PropertyId == propertyId);
    }

    public async Task<IEnumerable<Property>> GetWishlistPropertiesByTenantAsync(int tenantId)
    {
        return await _context.Wishlists
            .Where(w => w.TenantId == tenantId)
            .Include(w => w.Property)
            .Select(w => w.Property!)
            .ToListAsync();
    }

    public async Task AddAsync(Wishlist wishlist)
    {
        await _context.Wishlists.AddAsync(wishlist);
    }

    public void Delete(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
    }
}