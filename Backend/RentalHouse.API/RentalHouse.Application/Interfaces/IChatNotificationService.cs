using RentalHouse.Application.DTOs.Chat; 

namespace RentalHouse.Application.Interfaces;

public interface IChatNotificationService
{
    Task SendMessageToUserAsync(int receiverId, MessageDto message);
}