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
            Status = PropertyStatus.Pending // Mặc định chờ duyệt
        };

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