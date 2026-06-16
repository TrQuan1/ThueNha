using MediatR;

namespace RentalHouse.Application.Features.Users.Commands;

public class BanUserCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class UnbanUserCommand : IRequest<bool>
{
    public int Id { get; set; }
}