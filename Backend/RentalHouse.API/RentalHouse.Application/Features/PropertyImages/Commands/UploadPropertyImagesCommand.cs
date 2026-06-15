using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Features.PropertyImages.Commands;

// Đã xóa ImageUploadDto vì chúng ta không truyền Stream (file vật lý) qua đây nữa

public class UploadPropertyImagesCommand : IRequest<IEnumerable<PropertyImageDto>>
{
    public int PropertyId { get; set; }
    public int HostId { get; set; }
    // Chỉ nhận vào danh sách URL (chuỗi) đã được Controller lưu thành công
    public List<string> ImageUrls { get; set; } = new List<string>();
}

public class UploadPropertyImagesCommandHandler : IRequestHandler<UploadPropertyImagesCommand, IEnumerable<PropertyImageDto>>
{
    private readonly IPropertyImageRepository _imageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Không cần IFileStorageService ở đây nữa vì Controller đã lo việc lưu file
    public UploadPropertyImagesCommandHandler(
        IPropertyImageRepository imageRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _imageRepository = imageRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PropertyImageDto>> Handle(UploadPropertyImagesCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà.");
        if (property.HostId != request.HostId) throw new UnauthorizedAccessException("Bạn chỉ được tải ảnh cho căn nhà của chính mình.");

        if (!request.ImageUrls.Any()) throw new Exception("Không có dữ liệu ảnh nào được truyền xuống.");

        var uploadedImages = new List<PropertyImage>();

        // Duyệt qua danh sách URL và lưu vào Database
        for (int i = 0; i < request.ImageUrls.Count; i++)
        {
            var propertyImage = new PropertyImage
            {
                PropertyId = request.PropertyId,
                ImageUrl = request.ImageUrls[i],
                // Tự động set bức ảnh đầu tiên làm ảnh đại diện (Thumbnail)
                IsThumbnail = (i == 0)
            };

            await _imageRepository.AddAsync(propertyImage);
            uploadedImages.Add(propertyImage);
        }

        // Chốt giao dịch, lưu toàn bộ xuống MySQL
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return uploadedImages.Select(img => new PropertyImageDto
        {
            Id = img.Id,
            PropertyId = img.PropertyId,
            ImageUrl = img.ImageUrl
        });
    }
}