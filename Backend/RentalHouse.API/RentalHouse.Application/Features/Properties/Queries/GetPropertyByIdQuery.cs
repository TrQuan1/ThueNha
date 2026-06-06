using MediatR;
using RentalHouse.Application.DTOs.Properties;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPropertyByIdQuery : IRequest<PropertyDto?>
{
    public int Id { get; set; }

    public GetPropertyByIdQuery(int id)
    {
        Id = id;
    }
}