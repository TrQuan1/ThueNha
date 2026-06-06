using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Properties.Commands;
using RentalHouse.Application.Features.Properties.Queries;
using RentalHouse.Domain.Constants;
using System.Security.Claims;

namespace RentalHouse.API.Controllers;

[Route("api/properties")] // Đường dẫn API sẽ là: /api/properties
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Host)] // Ổ KHÓA BẢO MẬT: Chỉ có Token mang quyền Host mới được vào
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyCommand command)
    {
        try
        {
            // BẢO MẬT: Tự động trích xuất Id của chủ nhà từ JWT Token
            // User.FindFirstValue sẽ tự động móc nối vào cấu hình JWT mà em đã làm ở Bước 11
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (int.TryParse(userIdString, out int hostId))
            {
                // Ghi đè HostId vào command một cách an toàn
                command.HostId = hostId;
            }
            else
            {
                return Unauthorized(new { Error = "Không thể xác định danh tính từ Token." });
            }

            // Gửi yêu cầu vào hệ thống ruột (CQRS)
            var result = await _mediator.Send(command);

            return Ok(new { Message = "Đăng tin nhà thành công!", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet]
    [AllowAnonymous] // Ai cũng có thể xem danh sách nhà
    public async Task<IActionResult> GetAllProperties()
    {
        try
        {
            var query = new GetPropertiesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // Ai cũng có thể xem chi tiết nhà
    public async Task<IActionResult> GetPropertyById(int id)
    {
        try
        {
            var query = new GetPropertyByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { Message = "Không tìm thấy căn nhà này." });

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

            await _mediator.Send(command);
            return Ok(new { Message = "Cập nhật thông tin nhà thành công." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Error = ex.Message }); // Lỗi 403 Forbidden nếu sửa nhà người khác
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
    [AllowAnonymous] // Bất kỳ ai (chưa đăng nhập) cũng có thể tìm kiếm nhà
    public async Task<IActionResult> SearchProperties([FromQuery] SearchPropertiesQuery query)
    {
        try
        {
            // Nếu người dùng nhập linh tinh số trang, ta ép về chuẩn
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