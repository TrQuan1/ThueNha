using MediatR;
using RentalHouse.Application.DTOs.Users;

public class GetUsersQuery : IRequest<List<UserDto>> { public string? Search { get; set; } }