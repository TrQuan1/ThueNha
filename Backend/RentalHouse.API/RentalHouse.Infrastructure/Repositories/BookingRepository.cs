using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasOverlappingBookingAsync(int propertyId, DateTime checkIn, DateTime checkOut)
    {
        return await _dbSet.AnyAsync(b =>
            b.PropertyId == propertyId &&
            (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Approved) && // Bỏ qua lịch đã bị hủy/từ chối
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByTenantAsync(int tenantId)
    {
        return await _dbSet
            .Include(b => b.Property) // Kéo theo thông tin Property để hiển thị
            .Where(b => b.TenantId == tenantId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByHostAsync(int hostId)
    {
        return await _dbSet
            .Include(b => b.Property)
            .Include(b => b.Tenant) // Xem ai là người đặt
            .Where(b => b.Property.HostId == hostId) // Chỉ lấy các booking thuộc nhà của Host này
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}