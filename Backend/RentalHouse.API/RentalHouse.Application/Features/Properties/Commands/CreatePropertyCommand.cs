using MediatR;
using RentalHouse.Application.DTOs.Properties;

namespace RentalHouse.Application.Features.Properties.Commands;

public class CreatePropertyCommand : IRequest<PropertyDto>
{
    public int HostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxGuests { get; set; }
    public List<int> FacilityIds { get; set; } = new();
}