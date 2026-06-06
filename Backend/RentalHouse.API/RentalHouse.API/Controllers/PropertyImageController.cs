using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.PropertyImages.Commands;
using RentalHouse.Application.Features.PropertyImages.Queries;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

// ĐỔI BASE ROUTE THÀNH "api" ĐỂ TRÁNH ĐỤNG ĐỘ
[Route("api")]
[ApiController]
public class PropertyImageController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyImageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.Parse(userIdString!);
    }

    // ĐỊNH NGHĨA ROUTE TUYỆT ĐỐI TẠI ĐÂY
    [HttpPost("properties/{propertyId}/images")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> UploadImages(int propertyId, [FromForm] List<IFormFile> files)
    {
        try
        {
            var command = new UploadPropertyImagesCommand
            {
                PropertyId = propertyId,
                HostId = GetUserIdFromToken()
            };

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    command.Images.Add(new ImageUploadDto
                    {
                        FileName = file.FileName,
                        Content = file.OpenReadStream()
                    });
                }
            }

            var result = await _mediator.Send(command);
            return Ok(new { Message = "Tải ảnh lên thành công.", Data = result });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // ĐỊNH NGHĨA ROUTE TUYỆT ĐỐI TẠI ĐÂY
    [HttpDelete("properties/images/{id}")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            var command = new DeletePropertyImageCommand { Id = id, HostId = GetUserIdFromToken() };
            await _mediator.Send(command);
            return Ok(new { Message = "Đã xóa ảnh thành công." });
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }

    // ĐỊNH NGHĨA ROUTE TUYỆT ĐỐI TẠI ĐÂY
    [HttpGet("properties/{propertyId}/images")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImages(int propertyId)
    {
        try
        {
            var query = new GetPropertyImagesQuery { PropertyId = propertyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex) { return BadRequest(new { Error = ex.Message }); }
    }
}