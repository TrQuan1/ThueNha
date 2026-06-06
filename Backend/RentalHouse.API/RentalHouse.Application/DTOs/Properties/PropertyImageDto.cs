namespace RentalHouse.Application.DTOs.Properties;

public class PropertyImageDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}