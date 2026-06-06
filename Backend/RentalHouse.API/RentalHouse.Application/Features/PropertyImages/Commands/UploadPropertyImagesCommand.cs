using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Features.PropertyImages.Commands;

// DTO thuần túy không chứa IFormFile
public class ImageUploadDto
{
    public Stream Content { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}

public class UploadPropertyImagesCommand : IRequest<IEnumerable<PropertyImageDto>>
{
    public int PropertyId { get; set; }
    public int HostId { get; set; }
    public List<ImageUploadDto> Images { get; set; } = new List<ImageUploadDto>();
}

public class UploadPropertyImagesCommandHandler : IRequestHandler<UploadPropertyImagesCommand, IEnumerable<PropertyImageDto>>
{
    private readonly IPropertyImageRepository _imageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService; // Sử dụng Service thay vì IWebHostEnvironment

    public UploadPropertyImagesCommandHandler(IPropertyImageRepository imageRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _imageRepository = imageRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<IEnumerable<PropertyImageDto>> Handle(UploadPropertyImagesCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà.");
        if (property.HostId != request.HostId) throw new UnauthorizedAccessException("Bạn chỉ được tải ảnh cho căn nhà của chính mình.");

        if (!request.Images.Any()) throw new Exception("Vui lòng chọn ít nhất 1 ảnh.");

        var uploadedImages = new List<PropertyImage>();

        foreach (var file in request.Images)
        {
            // Ủy quyền việc lưu file cho Infrastructure
            var fileUrl = await _fileStorageService.SaveFileAsync(file.Content, file.FileName);

            var propertyImage = new PropertyImage
            {
                PropertyId = request.PropertyId,
                ImageUrl = fileUrl
            };

            await _imageRepository.AddAsync(propertyImage);
            uploadedImages.Add(propertyImage);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return uploadedImages.Select(img => new PropertyImageDto
        {
            Id = img.Id,
            PropertyId = img.PropertyId,
            ImageUrl = img.ImageUrl
        });
    }
}