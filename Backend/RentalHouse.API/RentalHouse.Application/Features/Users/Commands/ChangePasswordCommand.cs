using MediatR;
using RentalHouse.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Users.Commands;

public class ChangePasswordCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher; // Dùng đúng vũ khí xịn của bạn
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra 2 ô mật khẩu mới phải khớp nhau
        if (request.NewPassword != request.ConfirmNewPassword)
            throw new Exception("Mật khẩu mới và xác nhận mật khẩu không khớp.");

        if (request.NewPassword.Length < 6)
            throw new Exception("Mật khẩu mới phải có ít nhất 6 ký tự.");

        // 2. Tìm User trong Database
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new Exception("Không tìm thấy thông tin người dùng.");

        // 3. Kiểm tra Mật khẩu cũ người dùng nhập vào có đúng không
        bool isPasswordCorrect = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!isPasswordCorrect)
            throw new Exception("Mật khẩu hiện tại không chính xác.");

        // 4. Mã hóa Mật khẩu mới và Lưu vào DB
        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}