using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class PropertyFacility
    {
        public int PropertyId { get; set; }
        public int FacilityId { get; set; }

        public Property Property { get; set; } = null!;
        public Facility Facility { get; set; } = null!;
    }
}
