using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.PropertyImages.Commands;
using RentalHouse.Application.Features.PropertyImages.Queries;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

[Route("api")]
[ApiController]
public class PropertyImageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorage;

    public PropertyImageController(IMediator mediator, IFileStorageService fileStorage)
    {
        _mediator = mediator;
        _fileStorage = fileStorage;
    }

    private int GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.Parse(userIdString!);
    }

    [HttpPost("properties/{propertyId}/images")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> UploadImages(int propertyId, [FromForm] List<IFormFile> files)
    {
        try
        {
            var imageUrls = new List<string>();

            // 1. Lưu từng file vật lý vào ổ cứng một cách an toàn (Tránh Crash Server)
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var readStream = file.OpenReadStream())
                    {
                        var fileUrl = await _fileStorage.SaveFileAsync(readStream, file.FileName);
                        imageUrls.Add(fileUrl);
                    }
                }
            }

            // 2. Gom danh sách URL vừa lưu xong gửi xuống MediatR để lưu vào Database
            var command = new UploadPropertyImagesCommand
            {
                PropertyId = propertyId,
                HostId = GetUserIdFromToken(),
                ImageUrls = imageUrls // Dữ liệu cầu nối giữa ổ cứng và DB
            };

            var result = await _mediator.Send(command);

            return Ok(new { Message = "Tải ảnh lên thành công.", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

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
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

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
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}