using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPendingPropertiesQueryHandler : IRequestHandler<GetPendingPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPendingPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<List<PropertyDto>> Handle(GetPendingPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetAllAsync();

        var pendingProperties = properties
            .Where(p => p.Status == PropertyStatus.Pending)
            .OrderByDescending(p => p.Id) // Sắp xếp mới nhất lên đầu
            .Select(p => new PropertyDto
            {
                Id = p.Id,
                HostId = p.HostId,
                Title = p.Title,
                Description = p.Description,
                Address = p.Address,
                PricePerNight = p.PricePerNight,
                MaxGuests = p.MaxGuests
                // Lưu ý: Bổ sung map thêm ImageUrl hoặc Facilities nếu cần thiết cho danh sách Admin
            })
            .ToList();

        return pendingProperties;
    }
}