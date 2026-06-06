using MediatR;

namespace RentalHouse.Application.Features.Properties.Commands;

public class DeletePropertyCommand : IRequest<bool>
{
    public int Id { get; set; }
    public int HostId { get; set; } // Lấy từ Token
}