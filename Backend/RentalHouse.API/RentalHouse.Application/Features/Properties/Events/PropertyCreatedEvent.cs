using MediatR;

namespace RentalHouse.Application.Features.Properties.Events;

public class PropertyCreatedEvent : INotification
{
    public int PropertyId { get; set; }
    public int HostId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
}