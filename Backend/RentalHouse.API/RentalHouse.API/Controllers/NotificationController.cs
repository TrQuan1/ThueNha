using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Notifications.Commands;
using RentalHouse.Application.Features.Notifications.Queries;

namespace RentalHouse.API.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize] // Bắt buộc đăng nhập mới xem được thông báo
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator) => _mediator = mediator;

    private int GetUserIdFromToken() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await _mediator.Send(new GetMyNotificationsQuery { UserId = GetUserIdFromToken() });
        return Ok(result);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _mediator.Send(new MarkNotificationAsReadCommand { NotificationId = id, UserId = GetUserIdFromToken() });
        return Ok(new { Message = "Đã đánh dấu là đã đọc." });
    }
}