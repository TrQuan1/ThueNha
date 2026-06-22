using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Chat.Commands;

// Định nghĩa Bưu kiện (Command) nhận vào Id phòng chat và Id người đọc
public class MarkMessagesAsReadCommand : IRequest<bool>
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }
}

// Bộ phận xử lý nghiệp vụ (Handler) - Nơi duy nhất được phép gọi Repository
public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, bool>
{
    private readonly IRepository<RentalHouse.Domain.Entities.Message> _messageRepo;
    private readonly IUnitOfWork _unitOfWork;

    public MarkMessagesAsReadCommandHandler(
        IRepository<RentalHouse.Domain.Entities.Message> messageRepo,
        IUnitOfWork unitOfWork)
    {
        _messageRepo = messageRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        var unreadMessages = await _messageRepo.GetAsync(
            predicate: m => m.ConversationId == request.ConversationId &&
                            m.SenderId != request.UserId &&
                            m.IsRead == false,
            disableTracking: false 
        );

        if (!unreadMessages.Any())
            return true;

        // Chuyển trạng thái sang Đã đọc
        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
            // _messageRepo.Update(msg); // Thực ra có tracking rồi thì không cần dòng Update này nữa, nhưng cứ để cũng không sao
        }

        // Xác nhận lưu xuống Database thông qua Unit of Work
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}