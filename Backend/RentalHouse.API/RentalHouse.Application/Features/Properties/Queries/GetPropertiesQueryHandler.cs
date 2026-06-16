using MediatR;
using RentalHouse.Application.DTOs.Properties;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;
// Tầng Application CHỈ using Domain và Interfaces, tuyệt đối không using EntityFrameworkCore hoặc Infrastructure

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<List<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        // 1. Gọi dữ liệu qua Interface Repository (Tuân thủ Clean Architecture)
        // Hàm GetAllAsync này đã được Override ở bài trước để lấy kèm p.Images và loại bỏ các bản ghi bị xóa mềm.
        var allProperties = await _propertyRepository.GetAllAsync();

        // 2. Khởi tạo Query trên bộ nhớ (LINQ to Objects)
        var query = allProperties.Where(p => p.Status == PropertyStatus.Active);

        // 3. Áp dụng bộ lọc động
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                (p.Title != null && p.Title.ToLower().Contains(searchTerm)) ||
                (p.Address != null && p.Address.ToLower().Contains(searchTerm)));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.PricePerNight >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.PricePerNight <= request.MaxPrice.Value);
        }

        // 4. Kết xuất sang DTO bằng .ToList() tiêu chuẩn (Không dùng ToListAsync() của EF Core)
        var properties = query
            .OrderByDescending(p => p.CreatedAt) // Mới nhất lên đầu
            .Select(p => new PropertyDto
            {
                Id = p.Id,
                HostId = p.HostId,
                Title = p.Title,
                Description = p.Description,
                Address = p.Address,
                PricePerNight = p.PricePerNight,
                MaxGuests = p.MaxGuests,
                ImageUrl = p.Images?.OrderByDescending(img => img.Id).FirstOrDefault()?.ImageUrl
            })
            .ToList();

        return properties;
    }
}