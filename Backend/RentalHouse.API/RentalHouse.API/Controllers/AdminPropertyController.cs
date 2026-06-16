using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Properties.Commands;
using RentalHouse.Application.Features.Properties.Queries;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

[Route("api/admin/properties")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]
public class AdminPropertyController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPropertyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingProperties()
    {
        try
        {
            var result = await _mediator.Send(new GetPendingPropertiesQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveProperty(int id)
    {
        try
        {
            var command = new ApprovePropertyCommand { PropertyId = id };
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Đã duyệt bài đăng thành công." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectProperty(int id, [FromBody] RejectPropertyRequest request)
    {
        try
        {
            var command = new RejectPropertyCommand
            {
                PropertyId = id,
                Reason = request.Reason
            };
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Đã từ chối bài đăng." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

// Lớp phụ trợ nhận payload cho lệnh Reject
public class RejectPropertyRequest
{
    public string Reason { get; set; } = string.Empty;
}