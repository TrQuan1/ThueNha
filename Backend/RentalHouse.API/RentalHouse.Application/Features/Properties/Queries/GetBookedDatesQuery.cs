using MediatR;
using System.Collections.Generic;

namespace RentalHouse.Application.Features.Properties.Queries;

public class GetBookedDatesQuery : IRequest<List<string>>
{
    public int PropertyId { get; set; }
}