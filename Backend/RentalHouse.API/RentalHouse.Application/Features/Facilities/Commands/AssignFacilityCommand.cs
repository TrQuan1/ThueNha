using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Features.Facilities.Commands;

public class AssignFacilityCommand : IRequest<bool>
{
    public int PropertyId { get; set; }
    public int FacilityId { get; set; }
    public int HostId { get; set; }
}

public class AssignFacilityCommandHandler : IRequestHandler<AssignFacilityCommand, bool>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignFacilityCommandHandler(IFacilityRepository facilityRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AssignFacilityCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà.");
        if (property.HostId != request.HostId) throw new UnauthorizedAccessException("Bạn chỉ được thêm tiện ích cho căn nhà của mình.");

        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId);
        if (facility == null) throw new Exception("Tiện ích này không tồn tại trên hệ thống.");

        if (await _facilityRepository.HasFacilityAsync(request.PropertyId, request.FacilityId))
            throw new Exception("Căn nhà này đã được gán tiện ích này rồi.");

        var pf = new PropertyFacility
        {
            PropertyId = request.PropertyId,
            FacilityId = request.FacilityId
        };

        await _facilityRepository.AssignFacilityAsync(pf);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}