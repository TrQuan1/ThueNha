namespace RentalHouse.Application.DTOs.Users;

public class ProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int Role { get; set; } // Ép về số nguyên theo yêu cầu
}