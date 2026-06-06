using RentalHouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public int TenantId { get; set; }
        public int HostId { get; set; }
        public int PropertyId { get; set; }
        public string? LastMessage { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public User Tenant { get; set; } = null!;
        public User Host { get; set; } = null!;
        public Property Property { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
