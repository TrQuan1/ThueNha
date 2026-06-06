using MediatR;

namespace RentalHouse.Application.Features.Properties.Commands;

public class UpdatePropertyCommand : IRequest<bool>
{
    public int Id { get; set; }
    public int HostId { get; set; } // Lấy từ Token để kiểm tra quyền sở hữu
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxGuests { get; set; }
}