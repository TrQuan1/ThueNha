using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Users.Commands;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin tài khoản.");
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;

        _userRepository.Update(user);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}