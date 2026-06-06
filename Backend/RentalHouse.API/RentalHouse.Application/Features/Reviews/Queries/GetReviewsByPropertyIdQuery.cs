using MediatR;
using RentalHouse.Application.DTOs.Reviews;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Reviews.Queries;

public class GetReviewsByPropertyIdQuery : IRequest<IEnumerable<ReviewDto>>
{
    public int PropertyId { get; set; }
}

public class GetReviewsByPropertyIdQueryHandler : IRequestHandler<GetReviewsByPropertyIdQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByPropertyIdQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByPropertyIdQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByPropertyIdAsync(request.PropertyId);

        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            BookingId = r.BookingId,
            TenantId = r.TenantId,
            TenantName = r.Tenant?.FullName ?? "Người dùng ẩn danh",
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }
}