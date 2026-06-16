using MediatR;
using RentalHouse.Application.DTOs.Users;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Users.Queries;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ProfileDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin tài khoản.");
        }

        return new ProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = (int)user.Role // Ép kiểu Enum sang int
        };
    }
}