using RentalHouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Facility : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;

        public ICollection<PropertyFacility> PropertyFacilities { get; set; } = new List<PropertyFacility>();
    }
}
