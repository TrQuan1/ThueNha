using MediatR;
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
}