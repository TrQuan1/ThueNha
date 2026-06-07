using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
}