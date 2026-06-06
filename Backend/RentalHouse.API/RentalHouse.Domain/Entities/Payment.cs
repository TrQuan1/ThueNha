using RentalHouse.Domain.Common;
using RentalHouse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }

        public Booking Booking { get; set; } = null!;
    }
}
