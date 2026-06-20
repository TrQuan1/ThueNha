using System.Threading.Tasks;

namespace RentalHouse.Application.Interfaces;

public interface INotificationSender
{
    Task SendAsync(int userId, string title, string content, string redirectUrl);
}