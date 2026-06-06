using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id);

        if (property == null) return null;

        return new PropertyDto
        {
            Id = property.Id,
            HostId = property.HostId,
            Title = property.Title,
            Description = property.Description,
            Address = property.Address,
            PricePerNight = property.PricePerNight,
            MaxGuests = property.MaxGuests
        };
    }
}