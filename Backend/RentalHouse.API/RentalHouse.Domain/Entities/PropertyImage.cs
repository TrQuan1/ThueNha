using RentalHouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class PropertyImage : BaseEntity
    {
        public int PropertyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsThumbnail { get; set; }

        // Navigation property
        public Property? Property { get; set; }
    }
}
