using MediatR;
using RentalHouse.Application.DTOs.Auth;

namespace RentalHouse.Application.Features.Auth.Commands;

// Frontend sẽ gửi Email và Password, mong đợi nhận lại AuthResponseDto (chứa Token)
public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}