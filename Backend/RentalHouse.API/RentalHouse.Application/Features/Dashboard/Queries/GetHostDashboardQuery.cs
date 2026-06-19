using MediatR;
using RentalHouse.Application.DTOs.Dashboard;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Dashboard.Queries;

public class GetHostDashboardQuery : IRequest<HostDashboardDto>
{
    public int HostId { get; set; }
    public int? Month { get; set; } // Thêm tham số Tháng
    public int? Year { get; set; }  // Thêm tham số Năm
}

public class GetHostDashboardQueryHandler : IRequestHandler<GetHostDashboardQuery, HostDashboardDto>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IBookingRepository _bookingRepository;

    public GetHostDashboardQueryHandler(
        IPropertyRepository propertyRepository,
        IBookingRepository bookingRepository)
    {
        _propertyRepository = propertyRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<HostDashboardDto> Handle(GetHostDashboardQuery request, CancellationToken cancellationToken)
    {
        // 1. Xác định tháng/năm cần lọc (Nếu không truyền thì lấy hiện tại)
        var targetMonth = request.Month ?? DateTime.UtcNow.Month;
        var targetYear = request.Year ?? DateTime.UtcNow.Year;

        // 2. Lấy Tổng số nhà (Cái này thì lúc nào cũng tính tất cả)
        var totalProperties = await _propertyRepository.CountPropertiesByHostIdAsync(request.HostId);

        // 3. Lấy Booking của Host & Lọc theo Tháng/Năm đã chọn
        var allBookings = await _bookingRepository.GetBookingsForHostAsync(request.HostId);
        var monthlyBookings = allBookings
            .Where(b => b.CheckInDate.Month == targetMonth && b.CheckInDate.Year == targetYear)
            .ToList();

        // 4. Các chỉ số cũng nhảy theo tháng
        var totalGuests = monthlyBookings.Sum(b => b.NumberOfGuests);
        var totalRevenue = monthlyBookings.Sum(b => b.TotalPrice);

        // 5. Vẽ biểu đồ
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

        return new HostDashboardDto
        {
            TotalProperties = totalProperties,
            TotalGuests = totalGuests,
            TotalRevenue = totalRevenue,
            RevenueChart = revenueChart
        };
    }
}