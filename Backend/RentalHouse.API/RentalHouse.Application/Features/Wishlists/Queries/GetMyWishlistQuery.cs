using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Wishlists.Queries;

public class GetMyWishlistQuery : IRequest<IEnumerable<PropertyDto>>
{
    public int TenantId { get; set; }
}

public class GetMyWishlistQueryHandler : IRequestHandler<GetMyWishlistQuery, IEnumerable<PropertyDto>>
{
    private readonly IWishlistRepository _wishlistRepository;

    public GetMyWishlistQueryHandler(IWishlistRepository wishlistRepository)
    {
        _wishlistRepository = wishlistRepository;
    }

    public async Task<IEnumerable<PropertyDto>> Handle(GetMyWishlistQuery request, CancellationToken cancellationToken)
    {
        var properties = await _wishlistRepository.GetWishlistPropertiesByTenantAsync(request.TenantId);

        return properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            HostId = p.HostId,
            Title = p.Title,
            Description = p.Description,
            Address = p.Address,
            PricePerNight = p.PricePerNight,
            MaxGuests = p.MaxGuests,
            ImageUrl = p.Images?.Select(i => i.ImageUrl).FirstOrDefault() // Lấy URL bức ảnh đầu tiên
        });
    }
}