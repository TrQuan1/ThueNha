using MediatR;

namespace RentalHouse.Application.Features.Properties.Commands;

public class RejectPropertyCommand : IRequest<bool>
{
    public int PropertyId { get; set; }
    public string Reason { get; set; } = string.Empty;
}