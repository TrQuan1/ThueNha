using MediatR;
using RentalHouse.Application.DTOs.Notifications;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Notifications.Queries;

public class GetMyNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public int UserId { get; set; }
}

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    public GetMyNotificationsQueryHandler(INotificationRepository repository) => _repository = repository;

    public async Task<IEnumerable<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetByUserIdAsync(request.UserId);
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,       // Thêm Title
            Content = n.Content,   // Sửa từ Message thành Content
            RedirectUrl = n.RedirectUrl,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        });
    }
}