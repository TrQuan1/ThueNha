using MediatR;
using RentalHouse.Application.DTOs.Users;

namespace RentalHouse.Application.Features.Users.Queries;

public class GetUserProfileQuery : IRequest<ProfileDto>
{
    public int UserId { get; set; }
}