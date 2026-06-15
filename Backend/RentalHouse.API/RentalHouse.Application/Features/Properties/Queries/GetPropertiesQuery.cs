using MediatR;
using RentalHouse.Application.DTOs.Properties;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPropertiesQuery : IRequest<List<PropertyDto>>
{
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}