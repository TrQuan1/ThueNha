using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Facilities.Commands;
using RentalHouse.Application.Features.Facilities.Queries;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

[Route("api")]
[ApiController]
public class FacilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.TryParse(userIdString, out var id) ? id : 0;
    }

    // 1. Lấy tất cả danh mục tiện ích (Ai cũng xem được)
    [HttpGet("facilities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllFacilities()
    {
        try
        {
            return Ok(await _mediator.Send(new GetAllFacilitiesQuery()));
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 2. Admin tạo tiện ích gốc
    [HttpPost("facilities")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> CreateFacility([FromBody] CreateFacilityCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { Message = "Đã tạo tiện ích thành công", Data = result });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 3. Xem tiện ích của 1 căn nhà
    [HttpGet("properties/{propertyId}/facilities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPropertyFacilities(int propertyId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetFacilitiesByPropertyQuery { PropertyId = propertyId }));
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 4. Host gán tiện ích cho nhà
    [HttpPost("properties/{propertyId}/facilities/{facilityId}")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> AssignFacility(int propertyId, int facilityId)
    {
        try
        {
            var command = new AssignFacilityCommand
            {
                PropertyId = propertyId,
                FacilityId = facilityId,
                HostId = GetUserIdFromToken()
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã gán tiện ích thành công." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // 5. Host gỡ tiện ích
    [HttpDelete("properties/{propertyId}/facilities/{facilityId}")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> RemoveFacility(int propertyId, int facilityId)
    {
        try
        {
            var command = new RemoveFacilityCommand
            {
                PropertyId = propertyId,
                FacilityId = facilityId,
                HostId = GetUserIdFromToken()
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã gỡ tiện ích thành công." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
}