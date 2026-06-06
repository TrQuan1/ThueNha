using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Wishlist
    {
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Tenant { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}
