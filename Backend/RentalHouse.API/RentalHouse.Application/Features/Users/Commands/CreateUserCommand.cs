using MediatR;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Users.Commands;

// Command yêu cầu tạo User và mong đợi trả về Id (kiểu int) của User vừa tạo
public class CreateUserCommand : IRequest<int>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Role Role { get; set; }
}