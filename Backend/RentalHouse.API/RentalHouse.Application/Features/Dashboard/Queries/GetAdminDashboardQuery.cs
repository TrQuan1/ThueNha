using MediatR;
using RentalHouse.Application.DTOs.Dashboard;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Dashboard.Queries;

// 2. Query
public class GetAdminDashboardQuery : IRequest<AdminDashboardDto>
{
    public int? Month { get; set; }
    public int? Year { get; set; }
}

// 3. Handler
public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;

    public GetAdminDashboardQueryHandler(
        IPropertyRepository propertyRepository,
        IBookingRepository bookingRepository,
        IUserRepository userRepository)
    {
        _propertyRepository = propertyRepository;
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
    }

    public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var targetMonth = request.Month ?? DateTime.UtcNow.Month;
        var targetYear = request.Year ?? DateTime.UtcNow.Year;

        // 1. Đếm tổng User và Nhà trên TOÀN HỆ THỐNG
        var allUsers = await _userRepository.GetAllAsync();
        var customerCount = allUsers.Count(u => u.Role != Role.Admin);
        var allProperties = await _propertyRepository.GetAllAsync();
        var validPropertiesCount = allProperties.Count(p =>
    !p.IsDeleted && p.Status == PropertyStatus.Active);

        // 2. Lấy toàn bộ Booking trên TOÀN HỆ THỐNG
        var allBookings = await _bookingRepository.GetAllAsync();

        // Lọc Booking theo tháng và chỉ lấy trạng thái thành công/đã duyệt
        var monthlyBookings = allBookings
        .Where(b => !b.IsDeleted
             && b.CheckInDate.Month == targetMonth
             && b.CheckInDate.Year == targetYear
             && (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Completed))
        .ToList();

        // 3. Tính GMV (Tổng giá trị giao dịch)
        var totalGMV = monthlyBookings.Sum(b => b.TotalPrice);

        // 4. Vẽ biểu đồ
        var revenueChart = monthlyBookings
            .GroupBy(b => b.CheckInDate.Date)
            .Select(g => new DailyRevenueDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Revenue = g.Sum(b => b.TotalPrice),
                BookingCount = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        return new AdminDashboardDto
        {
            TotalUsers = customerCount,
            TotalProperties = validPropertiesCount,
            TotalBookings = monthlyBookings.Count,
            TotalGMV = totalGMV,
            RevenueChart = revenueChart
        };
    }
}