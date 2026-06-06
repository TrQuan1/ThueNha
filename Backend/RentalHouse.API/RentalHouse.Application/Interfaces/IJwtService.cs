using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IJwtService
{
    // Sinh ra chuỗi Access Token thời hạn ngắn (ví dụ: 15 phút)
    string GenerateAccessToken(User user);

    // Sinh ra chuỗi mã ngẫu nhiên làm Refresh Token thời hạn dài (ví dụ: 7 ngày)
    string GenerateRefreshToken();
}