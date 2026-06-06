using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    // Kiểm tra xem khoảng thời gian chọn có bị trùng với các Booking đã được duyệt hoặc đang chờ duyệt không
    Task<bool> HasOverlappingBookingAsync(int propertyId, DateTime checkIn, DateTime checkOut);

    // Lấy danh sách nhà mà khách đã đặt
    Task<IEnumerable<Booking>> GetBookingsByTenantAsync(int tenantId);

    // Lấy danh sách yêu cầu đặt phòng gửi đến các nhà của Host
    Task<IEnumerable<Booking>> GetBookingsByHostAsync(int hostId);
}