using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Reviews.Commands;

public class UpdateReviewCommand : IRequest<bool>
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository, IBookingRepository bookingRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5) throw new Exception("Điểm đánh giá phải từ 1 đến 5.");

        var review = await _reviewRepository.GetByIdAsync(request.Id);
        if (review == null) throw new Exception("Không tìm thấy đánh giá.");

        if (review.TenantId != request.TenantId) throw new UnauthorizedAccessException("Bạn không có quyền sửa đánh giá này.");

        review.Rating = request.Rating;
        review.Comment = request.Comment;
        _reviewRepository.Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Cập nhật lại Rating Property
        var booking = await _bookingRepository.GetByIdAsync(review.BookingId);
        var property = await _propertyRepository.GetByIdAsync(booking!.PropertyId);
        var stats = await _reviewRepository.CalculatePropertyRatingAsync(property!.Id);
        property.AverageRating = stats.Average;

        _propertyRepository.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}