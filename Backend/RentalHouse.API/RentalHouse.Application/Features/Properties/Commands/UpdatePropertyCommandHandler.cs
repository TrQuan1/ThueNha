using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Features.Properties.Commands;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePropertyCommandHandler(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id);

        if (property == null)
            throw new Exception("Không tìm thấy căn nhà này.");

        // BẢO MẬT: Kiểm tra xem người đang request có phải là chủ sở hữu không
        if (property.HostId != request.HostId)
            throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa căn nhà của người khác.");

        // Cập nhật thông tin
        property.Title = request.Title;
        property.Description = request.Description;
        property.Address = request.Address;
        property.PricePerNight = request.PricePerNight;
        property.MaxGuests = request.MaxGuests;


        property.PropertyFacilities.Clear();
        if (request.FacilityIds != null && request.FacilityIds.Any())
        {
            foreach (var facilityId in request.FacilityIds)
            {
                property.PropertyFacilities.Add(new PropertyFacility { FacilityId = facilityId });
            }
        }
        _propertyRepository.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}