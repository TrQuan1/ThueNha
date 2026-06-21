using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Auth.Commands;
using RentalHouse.Application.Features.Users.Commands;

namespace RentalHouse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
    {
        try
        {
            var userId = await _mediator.Send(command);
            return Ok(new { Message = "Đăng ký tài khoản thành công", UserId = userId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var authResponse = await _mediator.Send(command);
            return Ok(authResponse); // Trả về Token kèm thông tin User
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Error = ex.Message }); // Lỗi sai mật khẩu (HTTP 401)
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message }); // Các lỗi hệ thống khác (HTTP 400)
        }
    }
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            var command = new ForgotPasswordCommand { Email = request.Email };
            await _mediator.Send(command);
            return Ok(new { message = "Mã xác thực đã được gửi tới email của bạn." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var command = new ResetPasswordCommand
            {
                Email = request.Email,
                Otp = request.Otp,
                NewPassword = request.NewPassword
            };
            await _mediator.Send(command);
            return Ok(new { message = "Khôi phục mật khẩu thành công. Bạn có thể đăng nhập ngay." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    public class ForgotPasswordRequest { public string Email { get; set; } = string.Empty; }
    public class ResetPasswordRequest { public string Email { get; set; } = string.Empty; public string Otp { get; set; } = string.Empty; public string NewPassword { get; set; } = string.Empty; }
}