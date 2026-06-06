using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Properties.Commands;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyCommandHandler(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id);

        if (property == null)
            throw new Exception("Không tìm thấy căn nhà này.");

        if (property.HostId != request.HostId)
            throw new UnauthorizedAccessException("Bạn không có quyền xóa căn nhà của người khác.");

        // Gọi hàm Delete của Repository (sẽ tự động kích hoạt logic Soft Delete IsDeleted = true)
        _propertyRepository.Delete(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}