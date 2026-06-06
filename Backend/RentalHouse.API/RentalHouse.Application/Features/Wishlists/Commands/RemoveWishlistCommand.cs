using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Wishlists.Commands;

public class RemoveWishlistCommand : IRequest<bool>
{
    public int TenantId { get; set; }
    public int PropertyId { get; set; }
}

public class RemoveWishlistCommandHandler : IRequestHandler<RemoveWishlistCommand, bool>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWishlistCommandHandler(IWishlistRepository wishlistRepository, IUnitOfWork unitOfWork)
    {
        _wishlistRepository = wishlistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveWishlistCommand request, CancellationToken cancellationToken)
    {
        var wishlist = await _wishlistRepository.GetWishlistItemAsync(request.TenantId, request.PropertyId);
        if (wishlist == null) throw new Exception("Căn nhà này chưa có trong danh sách yêu thích của bạn.");

        // Xóa đồng bộ
        _wishlistRepository.Delete(wishlist);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}