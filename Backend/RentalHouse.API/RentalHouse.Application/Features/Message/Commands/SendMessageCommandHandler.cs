using MediatR;
using RentalHouse.Application.DTOs.Chat;
using RentalHouse.Application.Features.Message.Commands;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    // Dùng tên tuyệt đối RentalHouse.Domain.Entities.Message để sửa lỗi "is a namespace"
    private readonly IRepository<RentalHouse.Domain.Entities.Message> _messageRepo;
    private readonly IRepository<Conversation> _conversationRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChatNotificationService _chatNotificationService;

    public SendMessageCommandHandler(
        IRepository<RentalHouse.Domain.Entities.Message> messageRepo,
        IRepository<Conversation> conversationRepo,
        IUnitOfWork unitOfWork,
        IChatNotificationService chatNotificationService)
    {
        _messageRepo = messageRepo;
        _conversationRepo = conversationRepo;
        _unitOfWork = unitOfWork;
        _chatNotificationService = chatNotificationService;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. Lưu tin nhắn
        var message = new RentalHouse.Domain.Entities.Message
        {
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content,
            IsRead = false
        };
        await _messageRepo.AddAsync(message);

        // 2. Cập nhật Conversation
        var conversation = await _conversationRepo.GetByIdAsync(request.ConversationId);
        if (conversation != null)
        {
            conversation.LastMessage = request.Content;
            conversation.LastUpdatedAt = DateTime.UtcNow;
            _conversationRepo.Update(conversation);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var messageDto = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            IsRead = false
        };

        // 3. Bắn SignalR thông qua Interface trừu tượng (Không dính dáng tới API)
        await _chatNotificationService.SendMessageToUserAsync(request.ReceiverId, messageDto);

        return messageDto;
    }
}