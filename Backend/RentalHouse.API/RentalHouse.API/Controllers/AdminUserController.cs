using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Domain.Constants;
using RentalHouse.Application.Features.Users.Commands;
using RentalHouse.Application.Features.Users.Queries;

[Route("api/admin/users")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]
public class AdminUserController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminUserController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
        => Ok(await _mediator.Send(new GetUsersQuery { Search = search }));

    [HttpPut("{id}/ban")]
    public async Task<IActionResult> Ban(int id)
        => Ok(await _mediator.Send(new BanUserCommand { Id = id }));

    [HttpPut("{id}/unban")]
    public async Task<IActionResult> Unban(int id)
        => Ok(await _mediator.Send(new UnbanUserCommand { Id = id }));
}