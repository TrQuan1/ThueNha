using MediatR;

namespace RentalHouse.Application.Features.Properties.Commands;

public class ApprovePropertyCommand : IRequest<bool>
{
    public int PropertyId { get; set; }
}