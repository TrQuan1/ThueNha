using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Commands;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, PropertyDto>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePropertyCommandHandler(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyDto> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = new Property
        {
            HostId = request.HostId,
            Title = request.Title,
            Description = request.Description,
            Address = request.Address,
            PricePerNight = request.PricePerNight,
            MaxGuests = request.MaxGuests,
            Status = PropertyStatus.Pending
        };

        // THÊM LOGIC TIỆN ÍCH Ở ĐÂY:
        // Khởi tạo danh sách PropertyFacilities ngay lập tức. 
        // EF Core sẽ tự động liên kết PropertyId sau khi lưu vào database.
        if (request.FacilityIds != null && request.FacilityIds.Any())
        {
            property.PropertyFacilities = request.FacilityIds.Select(facilityId => new PropertyFacility
            {
                FacilityId = facilityId
            }).ToList();
        }

        // Gọi AddAsync 1 lần duy nhất, EF Core sẽ tự động thêm cả Property và PropertyFacilities vào DB.
        await _propertyRepository.AddAsync(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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