using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Commands;

public class RejectPropertyCommandHandler : IRequestHandler<RejectPropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectPropertyCommandHandler(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RejectPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

        if (property == null)
        {
            throw new Exception($"Không tìm thấy bài đăng với ID: {request.PropertyId}");
        }

        property.Status = PropertyStatus.Rejected;

        // Bỏ comment dòng dưới nếu trong Entity Property của bạn có sẵn trường lưu lý do từ chối
        // property.RejectReason = request.Reason; 

        _propertyRepository.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}