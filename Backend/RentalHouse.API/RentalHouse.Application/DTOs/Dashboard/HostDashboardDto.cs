namespace RentalHouse.Application.DTOs.Dashboard;

public class HostDashboardDto
{
    public int TotalProperties { get; set; }
    public int TotalGuests { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<DailyRevenueDto> RevenueChart { get; set; } = new();
}

public class DailyRevenueDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int BookingCount { get; set; }
}