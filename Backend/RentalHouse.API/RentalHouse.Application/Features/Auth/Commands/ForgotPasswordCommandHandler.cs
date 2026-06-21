using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Auth.Commands;


public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;


    public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
            throw new Exception("Email này chưa được đăng ký trong hệ thống.");

        // Sinh OTP 6 số ngẫu nhiên
        var otp = new Random().Next(100000, 999999).ToString();

        // Lưu OTP và cho phép tồn tại trong 5 phút
        user.ResetPasswordOtp = otp;
        user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Gửi Email
        var emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #2563eb;'>Khôi phục mật khẩu RentalHouse</h2>
                <p>Xin chào <b>{user.FullName}</b>,</p>
                <p>Bạn vừa yêu cầu khôi phục mật khẩu. Dưới đây là mã xác thực OTP của bạn:</p>
                <div style='background-color: #f3f4f6; padding: 15px; text-align: center; border-radius: 8px; margin: 20px 0;'>
                    <span style='font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #1f2937;'>{otp}</span>
                </div>
                <p style='color: #dc2626; font-size: 14px;'>* Mã này chỉ có hiệu lực trong vòng 5 phút.</p>
                <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này để bảo vệ tài khoản.</p>
                <p>Trân trọng,<br>Đội ngũ RentalHouse</p>
            </div>";

        await _emailService.SendEmailAsync(user.Email, "Mã xác thực khôi phục mật khẩu", emailBody);

        return true;
    }
}