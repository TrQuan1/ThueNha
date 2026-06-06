using MediatR;
using RentalHouse.Application.DTOs.Users;

namespace RentalHouse.Application.Features.Users.Queries;

// Query truyền vào Id và mong muốn nhận lại UserDto
public class GetUserByIdQuery : IRequest<UserDto?>
{
    public int Id { get; set; }

    public GetUserByIdQuery(int id)
    {
        Id = id;
    }
}