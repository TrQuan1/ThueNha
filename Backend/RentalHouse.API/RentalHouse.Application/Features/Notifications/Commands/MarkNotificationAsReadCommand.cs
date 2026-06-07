using MediatR;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Notifications.Commands;

public class MarkNotificationAsReadCommand : IRequest<bool>
{
    public int NotificationId { get; set; }
    public int UserId { get; set; } // Dùng để kiểm tra quyền
}

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId);
        if (notification == null || notification.UserId != request.UserId)
            throw new UnauthorizedAccessException("Bạn không có quyền đánh dấu thông báo này.");

        await _repository.MarkAsReadAsync(request.NotificationId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}