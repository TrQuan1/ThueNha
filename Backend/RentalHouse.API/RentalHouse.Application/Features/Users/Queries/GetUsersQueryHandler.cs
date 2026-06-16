using MediatR;
using RentalHouse.Application.DTOs.Users;
using RentalHouse.Application.Interfaces;


public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;
    public GetUsersQueryHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync();

        var query = users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(u => u.FullName.Contains(request.Search) || u.Email.Contains(request.Search));

        return query.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            Status = u.Status
        }).ToList();
    }
}