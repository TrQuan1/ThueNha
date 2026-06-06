using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Features.Wishlists.Commands;

public class AddWishlistCommand : IRequest<bool>
{
    public int TenantId { get; set; }
    public int PropertyId { get; set; }
}

public class AddWishlistCommandHandler : IRequestHandler<AddWishlistCommand, bool>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWishlistCommandHandler(IWishlistRepository wishlistRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _wishlistRepository = wishlistRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddWishlistCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra nhà có tồn tại không
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null) throw new Exception("Không tìm thấy căn nhà này.");

        // 2. Validate chống trùng lặp
        var exists = await _wishlistRepository.ExistsAsync(request.TenantId, request.PropertyId);
        if (exists) throw new Exception("Căn nhà này đã có trong danh sách yêu thích của bạn.");

        // 3. Lưu vào Database
        var wishlist = new Wishlist
        {
            TenantId = request.TenantId,
            PropertyId = request.PropertyId
        };

        await _wishlistRepository.AddAsync(wishlist);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}