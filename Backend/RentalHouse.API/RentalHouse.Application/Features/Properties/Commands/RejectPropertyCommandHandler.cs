using MediatR;
using RentalHouse.Application.Features.Properties.Events;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Properties.Commands;

public class RejectPropertyCommandHandler : IRequestHandler<RejectPropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public RejectPropertyCommandHandler(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
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


        await _mediator.Publish(new PropertyStatusChangedEvent
        {
            PropertyId = property.Id,
            HostId = property.HostId,
            PropertyTitle = property.Title,
            TargetStatus = property.Status // Truyền PropertyStatus.Active hoặc Rejected
        }, cancellationToken);

        return true;
    }
}