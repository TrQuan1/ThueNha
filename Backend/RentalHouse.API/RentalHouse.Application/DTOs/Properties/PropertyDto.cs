using RentalHouse.Application.DTOs.Facilities;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.DTOs.Properties;

public class PropertyDto
{
    public int Id { get; set; }
    public int HostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxGuests { get; set; }

    // THÊM DÒNG NÀY ĐỂ TRẢ VỀ ẢNH ĐẠI DIỆN
    public string? ImageUrl { get; set; }
    public PropertyStatus Status { get; set; }
    public List<FacilityDto> Facilities { get; set; } = new();
}