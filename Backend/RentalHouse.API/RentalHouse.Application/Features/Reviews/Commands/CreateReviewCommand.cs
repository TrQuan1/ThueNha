using MediatR;
using RentalHouse.Application.DTOs.Reviews;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Reviews.Commands;

public class CreateReviewCommand : IRequest<ReviewDto>
{
    public int TenantId { get; set; }
    public int BookingId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRepository _notificationRepository;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IBookingRepository bookingRepository, IPropertyRepository propertyRepository, IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
    {
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Rating
        if (request.Rating < 1 || request.Rating > 5)
            throw new Exception("Điểm đánh giá phải từ 1 đến 5.");

        // 2. Validate Booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Không tìm thấy chuyến đi này.");

        if (booking.TenantId != request.TenantId)
            throw new UnauthorizedAccessException("Bạn chỉ được đánh giá chuyến đi của chính mình.");

        if (booking.Status != BookingStatus.Completed)
            throw new Exception("Chỉ được đánh giá khi chuyến đi đã hoàn thành.");

        // 3. Validate 1 Review / Booking
        if (await _reviewRepository.HasReviewedAsync(request.BookingId))
            throw new Exception("Bạn đã đánh giá chuyến đi này rồi.");

        // 4. Tạo Review
        var review = new Review
        {
            BookingId = request.BookingId,
            TenantId = request.TenantId,
            PropertyId = booking.PropertyId,
            Rating = request.Rating,
            Comment = request.Comment
        };
        await _reviewRepository.AddAsync(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // Lưu Review trước để có ID

        // 5. Cập nhật Rating cho Property
        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId);
        if (property != null)
        {
            var stats = await _reviewRepository.CalculatePropertyRatingAsync(property.Id);
            property.AverageRating = stats.Average;
            property.ReviewCount = stats.Count;
            _propertyRepository.Update(property);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        // 👉 CHÈN KHỐI TẠO THÔNG BÁO VÀO ĐÂY:
        if (property != null)
        {
            // Tạo tiếng sao lấp lánh cho tiêu đề dựa trên số điểm khách chấm
            string stars = new string('⭐', request.Rating);

            var notification = new Notification
            {
                UserId = property.HostId, // Gửi thẳng cho ông Chủ nhà
                Title = "Có đánh giá mới! " + stars,
                Content = $"Căn nhà '{property.Title}' của bạn vừa nhận được một đánh giá {request.Rating} sao từ khách hàng. Hãy vào xem ngay!",
                RedirectUrl = $"/properties/{property.Id}", // Bấm vào chuông thì nhảy tới trang nhà đó
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new ReviewDto
        {
            Id = review.Id,
            BookingId = review.BookingId,
            TenantId = review.TenantId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}