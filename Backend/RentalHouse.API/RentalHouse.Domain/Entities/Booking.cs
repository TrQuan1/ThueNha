using RentalHouse.Domain.Common;
using RentalHouse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }

        public User Tenant { get; set; } = null!;
        public Property Property { get; set; } = null!;
        public Payment? Payment { get; set; } // 1 Booking - 1 Payment
        public Review? Review { get; set; }   // 1 Booking - 1 Review
    }
}
