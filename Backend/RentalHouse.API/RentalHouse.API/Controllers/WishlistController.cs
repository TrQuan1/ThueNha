using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Wishlists.Commands;
using RentalHouse.Application.Features.Wishlists.Queries;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Tenant)] // CHÚ Ý: Gắn bảo mật ở mức class, mọi API bên trong đều phải là Tenant
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.Parse(userIdString!);
    }

    [HttpPost("{propertyId}")]
    public async Task<IActionResult> AddToWishlist(int propertyId)
    {
        try
        {
            var command = new AddWishlistCommand
            {
                TenantId = GetUserIdFromToken(),
                PropertyId = propertyId
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã thêm vào danh sách yêu thích." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    [HttpDelete("{propertyId}")]
    public async Task<IActionResult> RemoveFromWishlist(int propertyId)
    {
        try
        {
            var command = new RemoveWishlistCommand
            {
                TenantId = GetUserIdFromToken(),
                PropertyId = propertyId
            };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã gỡ khỏi danh sách yêu thích." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist()
    {
        try
        {
            var query = new GetMyWishlistQuery { TenantId = GetUserIdFromToken() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
}