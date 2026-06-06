using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

// KHÔNG kế thừa IRepository<Wishlist> nữa
public interface IWishlistRepository
{
    Task<bool> ExistsAsync(int tenantId, int propertyId);
    Task<Wishlist?> GetWishlistItemAsync(int tenantId, int propertyId);
    Task<IEnumerable<Property>> GetWishlistPropertiesByTenantAsync(int tenantId);

    // Tự định nghĩa 2 hàm cơ bản
    Task AddAsync(Wishlist wishlist);
    void Delete(Wishlist wishlist);
}