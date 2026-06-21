using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RentalHouse.Application.Interfaces;
using System.Security.Claims;

namespace RentalHouse.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatConnectionTracker _tracker;

    public ChatHub(IChatConnectionTracker tracker)
    {
        _tracker = tracker;
    }

    public override Task OnConnectedAsync()
    {
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? Context.User?.FindFirst("sub")?.Value;

        if (int.TryParse(userIdStr, out int userId))
        {
            _tracker.AddConnection(userId, Context.ConnectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? Context.User?.FindFirst("sub")?.Value;

        if (int.TryParse(userIdStr, out int userId))
        {
            _tracker.RemoveConnection(userId, Context.ConnectionId);
        }

        return base.OnDisconnectedAsync(exception);
    }
}