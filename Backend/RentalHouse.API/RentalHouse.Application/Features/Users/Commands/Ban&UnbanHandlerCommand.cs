using MediatR;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Enums;

public class BanUserCommand : IRequest<bool> { public int Id { get; set; } }
public class UnbanUserCommand : IRequest<bool> { public int Id { get; set; } }

public class UserStatusCommandHandler : IRequestHandler<BanUserCommand, bool>, IRequestHandler<UnbanUserCommand, bool>
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork _uow;
    public UserStatusCommandHandler(IUserRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<bool> Handle(BanUserCommand request, CancellationToken ct) => await UpdateStatus(request.Id, UserStatus.Banned, ct);
    public async Task<bool> Handle(UnbanUserCommand request, CancellationToken ct) => await UpdateStatus(request.Id, UserStatus.Active, ct);

    private async Task<bool> UpdateStatus(int id, UserStatus status, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(id) ?? throw new Exception("Người dùng không tồn tại.");
        user.Status = status;
        _repo.Update(user);
        return await _uow.SaveChangesAsync(ct) > 0;
    }
}