using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public Role Role { get; set; }
    public UserStatus Status { get; set; }
}