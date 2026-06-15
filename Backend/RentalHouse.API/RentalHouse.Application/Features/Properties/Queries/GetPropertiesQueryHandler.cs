using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, IEnumerable<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        // GỌI LẠI GetAllAsync() BÌNH THƯỜNG
        var properties = await _propertyRepository.GetAllAsync();

        return properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            HostId = p.HostId,
            Title = p.Title,
            Description = p.Description,
            Address = p.Address,
            PricePerNight = p.PricePerNight,
            MaxGuests = p.MaxGuests,
            ImageUrl = p.Images?.FirstOrDefault()?.ImageUrl
        });
    }
}