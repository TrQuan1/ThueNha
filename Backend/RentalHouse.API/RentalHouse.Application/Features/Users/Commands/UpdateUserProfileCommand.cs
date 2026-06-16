using MediatR;

namespace RentalHouse.Application.Features.Users.Commands;

public class UpdateUserProfileCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}