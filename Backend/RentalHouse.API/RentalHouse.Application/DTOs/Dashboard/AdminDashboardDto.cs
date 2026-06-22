using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Application.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalProperties { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalGMV { get; set; } // Tổng giá trị giao dịch (Gross Merchandise Volume)
        public List<DailyRevenueDto> RevenueChart { get; set; } = new();
    }
}
