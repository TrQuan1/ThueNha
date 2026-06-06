using RentalHouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        public User Tenant { get; set; } = null!;
        public Property Property { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }
}
