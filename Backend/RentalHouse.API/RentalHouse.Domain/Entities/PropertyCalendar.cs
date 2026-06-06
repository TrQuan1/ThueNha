using RentalHouse.Domain.Common;
using RentalHouse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class PropertyCalendar : BaseEntity
    {
        public int PropertyId { get; set; }
        public DateTime Date { get; set; }
        public CalendarStatus Status { get; set; }
        public decimal? PriceOverride { get; set; }

        public Property Property { get; set; } = null!;
    }
}
