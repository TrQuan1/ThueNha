using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Users.Commands;
using RentalHouse.Application.Features.Users.Queries;
using System.Security.Claims;

namespace RentalHouse.API.Controllers;

[Route("api/account/profile")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("Không thể xác minh danh tính người dùng.");
        }
        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _mediator.Send(new GetUserProfileQuery { UserId = userId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var command = new UpdateUserProfileCommand
            {
                UserId = userId,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };

            await _mediator.Send(command);
            return Ok(new { message = "Cập nhật thông tin thành công." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}