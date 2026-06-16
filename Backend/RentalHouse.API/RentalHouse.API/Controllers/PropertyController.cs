using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Properties.Commands;
using RentalHouse.Application.Features.Properties.Queries;
using RentalHouse.Domain.Constants;
using System.Security.Claims;

namespace RentalHouse.API.Controllers;

[Route("api/properties")] // Cấu hình Base Route cho toàn bộ Controller
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my-properties")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> GetMyProperties()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định danh tính người dùng." });
            }

            var query = new GetMyPropertiesQuery { UserId = userId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Đã di chuyển hàm vào bên trong class và gắn [FromQuery] hợp lệ
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProperties([FromQuery] GetPropertiesQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPropertyById(int id)
    {
        try
        {
            var query = new GetPropertyByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null) return NotFound(new { Message = "Không tìm thấy nhà." });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyCommand command)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int hostId))
                return Unauthorized(new { Error = "Không thể xác định danh tính." });

            command.HostId = hostId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> UpdateProperty(int id, [FromBody] UpdatePropertyCommand command)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int hostId))
                return Unauthorized(new { Error = "Không thể xác định danh tính." });

            command.Id = id;
            command.HostId = hostId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int hostId))
                return Unauthorized(new { Error = "Không thể xác định danh tính." });

            var command = new DeletePropertyCommand { Id = id, HostId = hostId };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã xóa nhà thành công." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchProperties([FromQuery] SearchPropertiesQuery query)
    {
        try
        {
            if (query.PageNumber < 1) query.PageNumber = 1;
            if (query.PageSize < 1 || query.PageSize > 50) query.PageSize = 10;

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}