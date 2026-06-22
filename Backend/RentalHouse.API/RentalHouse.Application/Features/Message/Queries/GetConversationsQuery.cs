using MediatR;
using RentalHouse.Application.DTOs.Chat;
using RentalHouse.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Chat.Queries
{
    public class GetConversationsQuery : IRequest<List<ConversationDto>>
    {
        public int UserId { get; set; }
    }

    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
    {
        private readonly IConversationRepository _conversationRepo;

        public GetConversationsQueryHandler(IConversationRepository conversationRepo)
        {
            _conversationRepo = conversationRepo;
        }

        public async Task<List<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            var conversations = await _conversationRepo.GetConversationsByUserIdAsync(request.UserId, cancellationToken);

            return conversations.Select(c => new ConversationDto
            {
                Id = c.Id,
                PropertyId = c.PropertyId,
                PropertyTitle = c.Property.Title,
                // Lấy đúng ID và Tên của người đang chat với mình
                PartnerId = c.TenantId == request.UserId ? c.HostId : c.TenantId,
                PartnerName = c.TenantId == request.UserId ? c.Host.FullName : c.Tenant.FullName,
                LastMessage = c.LastMessage,
                LastUpdatedAt = c.LastUpdatedAt,
                // Đếm số tin nhắn của "Người kia" gửi mà mình chưa đọc
                UnreadCount = c.Messages.Count(m => m.SenderId != request.UserId && m.IsRead == false)
            }).ToList();
        }
    }
}