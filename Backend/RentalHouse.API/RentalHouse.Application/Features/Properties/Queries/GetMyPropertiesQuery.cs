using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetMyPropertiesQuery : IRequest<List<PropertyDto>>
{
    public int UserId { get; set; }
}

public class GetMyPropertiesQueryHandler : IRequestHandler<GetMyPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetMyPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<List<PropertyDto>> Handle(GetMyPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetAllAsync();

        return properties
            .Where(p => p.HostId == request.UserId) // Nếu HostId của bạn là string, hãy dùng .ToString()
            .OrderByDescending(p => p.Id)
            .Select(p => new PropertyDto
            {
                Id = p.Id,
                HostId = p.HostId,
                Title = p.Title,
                Description = p.Description,
                Address = p.Address,
                PricePerNight = p.PricePerNight,
                MaxGuests = p.MaxGuests,
                Status = p.Status, // Giả sử DTO có chứa trường Status
                ImageUrl = p.Images?.FirstOrDefault()?.ImageUrl
            })
            .ToList();
    }
}