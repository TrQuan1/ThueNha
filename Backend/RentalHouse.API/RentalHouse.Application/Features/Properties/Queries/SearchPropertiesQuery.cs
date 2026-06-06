using MediatR;
using RentalHouse.Application.DTOs.Common;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Properties.Queries;

// Kế thừa IRequest trả về PagedResult thay vì List thông thường
public class SearchPropertiesQuery : IRequest<PagedResult<PropertyDto>>
{
    public string? Keyword { get; set; }
    public string? Location { get; set; } // Dùng cho City/Province
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinGuests { get; set; }
    public double? MinRating { get; set; }
    public string? SortBy { get; set; }
    public int PageNumber { get; set; } = 1;  // Mặc định trang 1
    public int PageSize { get; set; } = 10;   // Mặc định 10 item / trang
}

public class SearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, PagedResult<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public SearchPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResult<PropertyDto>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
    {
        // Gọi hàm Search ở Repository
        var (items, totalCount) = await _propertyRepository.SearchAsync(
            request.Keyword, request.Location, request.MinPrice, request.MaxPrice,
            request.MinGuests, request.MinRating, request.PageNumber, request.PageSize, request.SortBy);

        // Map sang DTO
        var propertyDtos = items.Select(p => new PropertyDto
        {
            Id = p.Id,
            HostId = p.HostId,
            Title = p.Title,
            Description = p.Description,
            Address = p.Address,
            PricePerNight = p.PricePerNight,
            MaxGuests = p.MaxGuests
        });

        // Đóng gói vào PagedResult
        return new PagedResult<PropertyDto>
        {
            Items = propertyDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}