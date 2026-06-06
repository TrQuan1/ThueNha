using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.PropertyImages.Queries;

public class GetPropertyImagesQuery : IRequest<IEnumerable<PropertyImageDto>>
{
    public int PropertyId { get; set; }
}

public class GetPropertyImagesQueryHandler : IRequestHandler<GetPropertyImagesQuery, IEnumerable<PropertyImageDto>>
{
    private readonly IPropertyImageRepository _imageRepository;

    public GetPropertyImagesQueryHandler(IPropertyImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public async Task<IEnumerable<PropertyImageDto>> Handle(GetPropertyImagesQuery request, CancellationToken cancellationToken)
    {
        var images = await _imageRepository.GetByPropertyIdAsync(request.PropertyId);
        return images.Select(img => new PropertyImageDto
        {
            Id = img.Id,
            PropertyId = img.PropertyId,
            ImageUrl = img.ImageUrl
        });
    }
}