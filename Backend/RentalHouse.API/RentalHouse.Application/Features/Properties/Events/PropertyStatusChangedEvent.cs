using MediatR;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Events;

public class PropertyStatusChangedEvent : INotification
{
    public int PropertyId { get; set; }
    public int HostId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public PropertyStatus TargetStatus { get; set; }
}