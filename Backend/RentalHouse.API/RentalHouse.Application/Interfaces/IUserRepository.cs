using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    // Hàm truy vấn đặc thù: Tìm User theo Email để phục vụ chức năng Login
    Task<User?> GetByEmailAsync(string email);
    // Thêm dòng này để định nghĩa việc tìm kiếm bằng Email
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}