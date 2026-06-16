using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Commands;

public class ApprovePropertyCommandHandler : IRequestHandler<ApprovePropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApprovePropertyCommandHandler(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ApprovePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

        if (property == null)
        {
            throw new Exception($"Không tìm thấy bài đăng với ID: {request.PropertyId}");
        }

        property.Status = PropertyStatus.Active;

        _propertyRepository.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}