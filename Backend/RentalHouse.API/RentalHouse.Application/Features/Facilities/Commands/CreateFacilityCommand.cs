using MediatR;
using RentalHouse.Application.DTOs.Facilities;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Features.Facilities.Commands;

public class CreateFacilityCommand : IRequest<FacilityDto>
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, FacilityDto>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFacilityCommandHandler(IFacilityRepository facilityRepository, IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityDto> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Tên tiện ích không được để trống.");

        var facility = new Facility
        {
            Name = request.Name,
            Icon = request.Icon
        };

        await _facilityRepository.AddAsync(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FacilityDto
        {
            Id = facility.Id,
            Name = facility.Name,
            Icon = facility.Icon
        };
    }
}