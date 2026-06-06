using RentalHouse.Domain.Common;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public Role Role { get; set; }
    public UserStatus Status { get; set; }

    // Các thuộc tính bổ sung để phục vụ cơ chế Refresh Token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation Properties
    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}