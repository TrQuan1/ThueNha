using Microsoft.AspNetCore.SignalR;
using RentalHouse.API.Hubs;
using RentalHouse.Application.DTOs.Chat;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.API.Services
{
    // Class này nằm ở tầng API, nên nó được phép gọi đến ChatHub và SignalR
    public class ChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatConnectionTracker _tracker;

        public ChatNotificationService(IHubContext<ChatHub> hubContext, IChatConnectionTracker tracker)
        {
            _hubContext = hubContext;
            _tracker = tracker;
        }

        // 👉 Đã xóa CancellationToken để khớp 100% với Interface của bạn
        public async Task SendMessageToUserAsync(int receiverId, MessageDto messageDto)
        {
            var receiverConnections = _tracker.GetConnections(receiverId).ToList();
            if (receiverConnections.Any())
            {
                await _hubContext.Clients.Clients(receiverConnections)
                                 .SendAsync("ReceiveMessage", messageDto);
            }
        }
    }
}