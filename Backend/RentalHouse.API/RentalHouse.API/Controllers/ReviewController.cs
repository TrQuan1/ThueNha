using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Reviews.Commands;
using RentalHouse.Application.Features.Reviews.Queries;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

[Route("api")] // Khai báo chung, cụ thể từng endpoint sẽ định nghĩa bên dưới
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.Parse(userIdString!);
    }

    [HttpPost("reviews")]
    [Authorize(Roles = AppRoles.Tenant)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
    {
        try
        {
            command.TenantId = GetUserIdFromToken();
            var result = await _mediator.Send(command);
            return Ok(new { Message = "Đánh giá thành công.", Data = result });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    [HttpPut("reviews/{id}")]
    [Authorize(Roles = AppRoles.Tenant)]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewCommand command)
    {
        try
        {
            command.Id = id;
            command.TenantId = GetUserIdFromToken();
            await _mediator.Send(command);
            return Ok(new { Message = "Cập nhật đánh giá thành công." });
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, new { Error = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    [HttpGet("properties/{propertyId}/reviews")]
    [AllowAnonymous] // Ai cũng xem được đánh giá của nhà
    public async Task<IActionResult> GetPropertyReviews(int propertyId)
    {
        try
        {
            var query = new GetReviewsByPropertyIdQuery { PropertyId = propertyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    [HttpDelete("reviews/{id}")]
    [Authorize(Roles = AppRoles.Tenant)] // Phải đăng nhập và là Tenant mới được xóa
    public async Task<IActionResult> DeleteReview(int id)
    {
        try
        {
            var command = new DeleteReviewCommand
            {
                Id = id,
                TenantId = GetUserIdFromToken()
            };

            await _mediator.Send(command);

            return Ok(new { Message = "Đã xóa đánh giá thành công." });
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
}