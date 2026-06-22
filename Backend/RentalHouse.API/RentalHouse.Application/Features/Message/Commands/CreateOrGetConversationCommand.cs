using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Chat.Commands;

// Yêu cầu chỉ cần trả về Id của Conversation (số nguyên)
public class CreateOrGetConversationCommand : IRequest<int>
{
    public int PropertyId { get; set; }
    public int HostId { get; set; }
    public int TenantId { get; set; }
}

public class CreateOrGetConversationCommandHandler : IRequestHandler<CreateOrGetConversationCommand, int>
{
    private readonly IRepository<Conversation> _conversationRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrGetConversationCommandHandler(IRepository<Conversation> conversationRepo, IUnitOfWork unitOfWork)
    {
        _conversationRepo = conversationRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateOrGetConversationCommand request, CancellationToken cancellationToken)
    {
        // 1. Dùng hàm mới: Truy vấn trực tiếp dưới Database, cực nhanh và không tốn RAM
        var conversation = await _conversationRepo.GetFirstOrDefaultAsync(c =>
            c.PropertyId == request.PropertyId &&
            c.TenantId == request.TenantId &&
            c.HostId == request.HostId,
            cancellationToken); // Truyền luôn cancellationToken để ngắt kết nối nếu cần

        // 2. Nếu có rồi, trả về luôn Id cũ (Không tạo mới)
        if (conversation != null)
        {
            return conversation.Id;
        }

        // 3. Nếu chưa có, tạo phòng chat mới
        var newConversation = new Conversation
        {
            PropertyId = request.PropertyId,
            TenantId = request.TenantId,
            HostId = request.HostId,
            LastUpdatedAt = DateTime.UtcNow,
            LastMessage = "Bắt đầu cuộc trò chuyện mới"
        };

        await _conversationRepo.AddAsync(newConversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newConversation.Id;
    }
}