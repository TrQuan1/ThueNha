using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.PropertyImages.Commands;

public class DeletePropertyImageCommand : IRequest<bool>
{
    public int Id { get; set; }
    public int HostId { get; set; }
}

public class DeletePropertyImageCommandHandler : IRequestHandler<DeletePropertyImageCommand, bool>
{
    private readonly IPropertyImageRepository _imageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeletePropertyImageCommandHandler(IPropertyImageRepository imageRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _imageRepository = imageRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<bool> Handle(DeletePropertyImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetByIdAsync(request.Id);
        if (image == null) throw new Exception("Không tìm thấy ảnh.");

        var property = await _propertyRepository.GetByIdAsync(image.PropertyId);
        if (property!.HostId != request.HostId) throw new UnauthorizedAccessException("Bạn không có quyền xóa ảnh này.");

        // Nhờ Infrastructure xóa file vật lý
        _fileStorageService.DeleteFile(image.ImageUrl);

        _imageRepository.Delete(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}