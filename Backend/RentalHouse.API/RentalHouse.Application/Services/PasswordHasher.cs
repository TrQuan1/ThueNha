using RentalHouse.Application.Interfaces;

namespace RentalHouse.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Sinh muối (salt) tự động và băm mật khẩu an toàn
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        // Đối chiếu mật khẩu người dùng nhập với mã Hash trong DB
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}