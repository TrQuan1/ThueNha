using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Auth.Commands;


public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
            throw new Exception("Tài khoản không tồn tại.");

        if (user.ResetPasswordOtp != request.Otp)
            throw new Exception("Mã OTP không chính xác.");

        if (user.OtpExpiryTime == null || user.OtpExpiryTime < DateTime.UtcNow)
            throw new Exception("Mã OTP đã hết hạn. Vui lòng gửi lại yêu cầu.");

        // Mã hóa mật khẩu mới
        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        // Reset lại OTP để tránh việc tái sử dụng
        user.ResetPasswordOtp = null;
        user.OtpExpiryTime = null;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}