using MediatR;
using RentalHouse.Application.DTOs.Users;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities; // Bổ sung using User
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Lưu ý: Nếu project của bạn bắt buộc có namespace, hãy giữ nguyên namespace ở đây

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        IEnumerable<User> users;

        // Dùng FindAsync để lọc ngay từ Database thay vì tải hết lên RAM
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            users = await _userRepository.FindAsync(u =>
                u.FullName.Contains(request.Search) || u.Email.Contains(request.Search));
        }
        else
        {
            users = await _userRepository.GetAllAsync();
        }

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            Status = u.Status
        }).ToList();
    }
}