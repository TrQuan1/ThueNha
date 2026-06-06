using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Domain.Constants;

namespace RentalHouse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestRolesController : ControllerBase
{
    [HttpGet("admin-endpoint")]
    [Authorize(Roles = AppRoles.Admin)] // Chỉ có token chứa Role Admin mới vào được
    public IActionResult GetAdminData()
    {
        return Ok(new { Message = "Truy cập thành công! Bạn có quyền của Quản Trị Viên (Admin)." });
    }

    [HttpGet("host-endpoint")]
    [Authorize(Roles = AppRoles.Host)] // Chỉ có token chứa Role Host mới vào được
    public IActionResult GetHostData()
    {
        return Ok(new { Message = "Truy cập thành công! Bạn có quyền của Chủ Nhà (Host)." });
    }

    [HttpGet("tenant-endpoint")]
    [Authorize(Roles = AppRoles.Tenant)] // Chỉ có token chứa Role Tenant mới vào được
    public IActionResult GetTenantData()
    {
        return Ok(new { Message = "Truy cập thành công! Bạn có quyền của Khách Thuê (Tenant)." });
    }

    [HttpGet("any-authenticated")]
    [Authorize] // Tài khoản nào đã đăng nhập (có token hợp lệ) đều vào được, không quan trọng vai trò
    public IActionResult GetCommonData()
    {
        return Ok(new { Message = "Truy cập thành công! Yêu cầu này chỉ cần bạn đã đăng nhập hệ thống." });
    }
}