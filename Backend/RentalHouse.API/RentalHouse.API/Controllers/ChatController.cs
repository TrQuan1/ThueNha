using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.DTOs.Chat;
using RentalHouse.Application.Features.Chat.Commands;
using RentalHouse.Application.Features.Chat.Queries;
using RentalHouse.Application.Features.Message.Commands;
using System.Security.Claims;

namespace RentalHouse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Bắt buộc phải đăng nhập
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    // Chỉ tiêm duy nhất IMediator, giữ Controller đúng vai trò tiếp tân mẫu mực
    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. API: Tạo hoặc lấy phòng chat giữa Khách và Chủ nhà
    [HttpPost("conversations")]
    public async Task<IActionResult> CreateOrGetConversation([FromBody] CreateConversationRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int currentUserId))
            return Unauthorized();

        var command = new CreateOrGetConversationCommand
        {
            PropertyId = request.PropertyId,
            HostId = request.HostId,
            TenantId = currentUserId
        };

        var conversationId = await _mediator.Send(command);
        return Ok(new { conversationId });
    }

    // 2. API: Lấy danh sách các cuộc hội thoại của người dùng hiện tại
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out int currentUserId))
            return Unauthorized();

        var query = new GetConversationsQuery { UserId = currentUserId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 3. API: Lấy lịch sử tin nhắn của một cuộc hội thoại
    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var query = new GetMessagesQuery { ConversationId = conversationId };
        var messageDtos = await _mediator.Send(query);
        return Ok(messageDtos);
    }

    // 4. API: Gửi tin nhắn mới
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out int currentUserId))
            return Unauthorized();

        var command = new SendMessageCommand
        {
            ConversationId = request.ConversationId,
            ReceiverId = request.ReceiverId,
            SenderId = currentUserId,
            Content = request.Content
        };

        var messageDto = await _mediator.Send(command);
        return Ok(messageDto);
    }

    // 5. API: Đánh dấu đã đọc toàn bộ tin nhắn trong phòng chat
    [HttpPut("conversations/{conversationId}/read")]
    public async Task<IActionResult> MarkMessagesAsRead(int conversationId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out int currentUserId))
            return Unauthorized();

        var command = new MarkMessagesAsReadCommand
        {
            ConversationId = conversationId,
            UserId = currentUserId
        };

        await _mediator.Send(command);
        return Ok();
    }
}

// --- CÁC CLASS REQUEST NẬM DỮ LIỆU TỪ FRONTEND ---
public class SendMessageRequest
{
    public int ConversationId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class CreateConversationRequest
{
    public int PropertyId { get; set; }
    public int HostId { get; set; }
}