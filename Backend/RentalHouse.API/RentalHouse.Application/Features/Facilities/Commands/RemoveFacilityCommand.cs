using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Facilities.Commands;

public class RemoveFacilityCommand : IRequest<bool>
{
    public int PropertyId { get; set; }
    public int FacilityId { get; set; }
    public int HostId { get; set; }
}

public class RemoveFacilityCommandHandler : IRequestHandler<RemoveFacilityCommand, bool>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFacilityCommandHandler(IFacilityRepository facilityRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveFacilityCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà.");
        if (property.HostId != request.HostId) throw new UnauthorizedAccessException("Bạn không có quyền gỡ tiện ích của căn nhà này.");

        var pf = await _facilityRepository.GetPropertyFacilityAsync(request.PropertyId, request.FacilityId);
        if (pf == null) throw new Exception("Căn nhà này chưa được gán tiện ích này.");

        await _facilityRepository.RemoveFacilityAsync(pf);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}