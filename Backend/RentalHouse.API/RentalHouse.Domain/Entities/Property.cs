using RentalHouse.Domain.Common;
using RentalHouse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Property : BaseEntity
    {
        public int HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public PropertyStatus Status { get; set; }
        public double AverageRating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;

        public User Host { get; set; } = null!;
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<PropertyFacility> PropertyFacilities { get; set; } = new List<PropertyFacility>();
        public ICollection<PropertyCalendar> Calendars { get; set; } = new List<PropertyCalendar>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Wishlist> WishlistedBy { get; set; } = new List<Wishlist>();
    }
}
