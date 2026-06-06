using System.Linq.Expressions;
using RentalHouse.Domain.Common;

namespace RentalHouse.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity); // Sẽ được cấu hình thành Xóa mềm
}