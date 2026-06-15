using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.PropertyImages.Commands;
using RentalHouse.Application.Features.PropertyImages.Queries;
using RentalHouse.Application.Interfaces; // Bổ sung namespace chứa IFileStorageService
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

// ĐỔI BASE ROUTE THÀNH "api" ĐỂ TRÁNH ĐỤNG ĐỘ
[Route("api")]
[ApiController]
public class PropertyImageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorage; // Inject service lưu trữ file

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

    // ĐỊNH NGHĨA ROUTE TUYỆT ĐỐI TẠI ĐÂY
    [HttpPost("properties/{propertyId}/images")]
    [Authorize(Roles = AppRoles.Host)]
    public async Task<IActionResult> UploadImages(int propertyId, [FromForm] List<IFormFile> files)
    {
        try
        {
            var imageUrls = new List<string>();

            // Xử lý stream I/O ngay tại rìa Controller để tránh crash
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // 1. Gói việc mở Stream vào khối 'using' để đảm bảo TỰ ĐỘNG ĐÓNG (Dispose) sau khi xong
                    using (var readStream = file.OpenReadStream())
                    {
                        // 2. Truyền stream đã được kiểm soát vào service
                        var fileUrl = await _fileStorage.SaveFileAsync(readStream, file.FileName);
                        imageUrls.Add(fileUrl);
                    }
                }
            }

            // Tạm thời trả về danh sách URL để bạn test. 
            // Nếu không còn crash, bạn có thể tạo một Command mới ở MediatR chỉ nhận vào List<string> ImageUrls để lưu vào DB.
            return Ok(new { Message = "Tải ảnh lên thành công.", Urls = imageUrls });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
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
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
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
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}