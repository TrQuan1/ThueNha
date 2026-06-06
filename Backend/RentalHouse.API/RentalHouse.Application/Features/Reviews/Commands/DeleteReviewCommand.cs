using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Reviews.Commands;

public class DeleteReviewCommand : IRequest<bool>
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandHandler(
        IReviewRepository reviewRepository,
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy Review
        var review = await _reviewRepository.GetByIdAsync(request.Id);
        if (review == null) throw new Exception("Không tìm thấy đánh giá.");

        // 2. Validate quyền sở hữu
        if (review.TenantId != request.TenantId)
            throw new UnauthorizedAccessException("Bạn không có quyền xóa đánh giá của người khác.");

        // 3. Lấy thông tin Booking và Property TRƯỚC KHI XÓA Review
        var booking = await _bookingRepository.GetByIdAsync(review.BookingId);
        var property = await _propertyRepository.GetByIdAsync(booking!.PropertyId);

        // 4. Xóa Review (Sử dụng Delete đồng bộ giống bài Property)
        _reviewRepository.Delete(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // Lần lưu 1: Xóa khỏi DB

        // 5. Tính toán và cập nhật lại AverageRating / ReviewCount cho Property
        var stats = await _reviewRepository.CalculatePropertyRatingAsync(property!.Id);
        property.AverageRating = stats.Average;
        property.ReviewCount = stats.Count;

        _propertyRepository.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // Lần lưu 2: Cập nhật nhà

        return true;
    }
}