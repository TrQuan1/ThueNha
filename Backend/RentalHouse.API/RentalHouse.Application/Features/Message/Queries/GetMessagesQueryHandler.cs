using MediatR;
using RentalHouse.Application.DTOs.Chat;
using RentalHouse.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Chat.Queries;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    // Handler mới là người được phép gọi Database
    private readonly IRepository<RentalHouse.Domain.Entities.Message> _messageRepo;

    public GetMessagesQueryHandler(IRepository<RentalHouse.Domain.Entities.Message> messageRepo)
    {
        _messageRepo = messageRepo;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepo.GetAsync(
            m => m.ConversationId == request.ConversationId,
            orderBy: q => q.OrderBy(m => m.CreatedAt)
        );

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            IsRead = m.IsRead
        }).ToList();
    }
}