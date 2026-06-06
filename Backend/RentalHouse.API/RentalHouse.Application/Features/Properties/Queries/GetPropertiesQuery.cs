using MediatR;
using RentalHouse.Application.DTOs.Properties;

namespace RentalHouse.Application.Features.Properties.Queries;

// Query này không cần truyền tham số đầu vào, mong đợi trả về một Danh sách (IEnumerable) các PropertyDto
public class GetPropertiesQuery : IRequest<IEnumerable<PropertyDto>>
{
}