using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Dashboard.Queries;
using RentalHouse.Domain.Constants;
using System.Security.Claims;

namespace RentalHouse.API.Controllers;

[Route("api/dashboard")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("host")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> GetHostDashboard([FromQuery] int? month, [FromQuery] int? year)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int hostId))
            {
                return Unauthorized(new { message = "Không thể xác định danh tính người dùng." });
            }

            // Truyền month và year vào Query
            var query = new GetHostDashboardQuery { HostId = hostId, Month = month, Year = year };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpGet("admin")]
    [Authorize(Roles = AppRoles.Admin)] // 👉 Chỉ Admin mới được gọi
    public async Task<IActionResult> GetAdminDashboard([FromQuery] int? month, [FromQuery] int? year)
    {
        try
        {
            var query = new GetAdminDashboardQuery { Month = month, Year = year };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}