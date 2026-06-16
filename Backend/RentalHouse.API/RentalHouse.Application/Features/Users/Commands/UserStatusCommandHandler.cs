using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

namespace RentalHouse.Application.Features.Users.Commands;

public class UserStatusCommandHandler :
    IRequestHandler<BanUserCommand, bool>,
    IRequestHandler<UnbanUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserStatusCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        return await UpdateStatus(request.Id, UserStatus.Banned, cancellationToken);
    }

    public async Task<bool> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
    {
        return await UpdateStatus(request.Id, UserStatus.Active, cancellationToken);
    }

    private async Task<bool> UpdateStatus(int id, UserStatus status, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException("Người dùng không tồn tại.");
        }

        user.Status = status;

        _userRepository.Update(user);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}