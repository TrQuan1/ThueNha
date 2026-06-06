using RentalHouse.Application.Interfaces;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Gọi hàm SaveChangesAsync đã được ghi đè (để tự update CreatedAt, UpdatedAt) ở ApplicationDbContext
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        // Giải phóng tài nguyên kết nối Database khi không còn sử dụng
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}