using MediatR;
using RentalHouse.Application.DTOs.Properties;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetPendingPropertiesQuery : IRequest<List<PropertyDto>>
{
}