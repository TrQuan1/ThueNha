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
        // Lấy toàn bộ danh sách nhà từ DB (Repository đã tự động áp dụng Soft Delete Filter)
        var properties = await _propertyRepository.GetAllAsync();

        // Map (chuyển đổi) từ Entity sang DTO để trả về cho Frontend
        return properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            HostId = p.HostId,
            Title = p.Title,
            Description = p.Description,
            Address = p.Address,
            PricePerNight = p.PricePerNight,
            MaxGuests = p.MaxGuests
        });
    }
}