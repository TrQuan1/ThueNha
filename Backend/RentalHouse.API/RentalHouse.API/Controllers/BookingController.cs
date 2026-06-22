using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Bookings.Commands;
using RentalHouse.Application.Features.Bookings.Queries;
using RentalHouse.Domain.Constants;
using RentalHouse.Domain.Enums;
using System.Security.Claims;

namespace RentalHouse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.Parse(userIdString!);
    }

    // 1. Tenant Đặt phòng
    [HttpPost]
    [Authorize(Roles = AppRoles.Tenant)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        try
        {
            command.TenantId = GetUserIdFromToken();
            var result = await _mediator.Send(command);
            return Ok(new { Message = "Đặt phòng thành công, vui lòng chờ Chủ nhà xác nhận.", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // 2. Tenant Hủy phòng
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = AppRoles.Tenant)]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            var command = new ChangeBookingStatusCommand
            {
                BookingId = id,
                UserId = GetUserIdFromToken(),
                TargetStatus = BookingStatus.Cancelled
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã hủy đặt phòng." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 3. Host Duyệt đặt phòng
    [HttpPut("{id}/approve")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> ApproveBooking(int id)
    {
        try
        {
            var command = new ChangeBookingStatusCommand
            {
                BookingId = id,
                UserId = GetUserIdFromToken(),
                TargetStatus = BookingStatus.Approved
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã phê duyệt yêu cầu đặt phòng." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 4. Host Từ chối đặt phòng
    [HttpPut("{id}/reject")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> RejectBooking(int id)
    {
        try
        {
            var command = new ChangeBookingStatusCommand
            {
                BookingId = id,
                UserId = GetUserIdFromToken(),
                TargetStatus = BookingStatus.Rejected
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã từ chối yêu cầu đặt phòng." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 5. Tenant xem lịch sử đặt phòng của mình
    [HttpGet("my-bookings")]
    [Authorize(Roles = AppRoles.Tenant)]
    public async Task<IActionResult> GetMyBookings()
    {
        try
        {
            var query = new GetTenantBookingsQuery { TenantId = GetUserIdFromToken() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 6. Host xem các yêu cầu đặt phòng tới nhà của mình
    [HttpGet("host-requests")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> GetHostBookings()
    {
        try
        {
            var query = new GetHostBookingsQuery { HostId = GetUserIdFromToken() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
    // 7. Host Hủy đặt phòng (Trường hợp khách No-show)
    [HttpPut("{id}/host-cancel")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> HostCancelBooking(int id)
    {
        try
        {
            var command = new ChangeBookingStatusCommand
            {
                BookingId = id,
                UserId = GetUserIdFromToken(),
                TargetStatus = BookingStatus.Cancelled
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã hủy đơn đặt phòng (Khách không đến)." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
}