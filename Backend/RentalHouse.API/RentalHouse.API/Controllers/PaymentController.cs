using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.Features.Payments.Commands;

[Route("api/payments")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentController(IMediator mediator) => _mediator = mediator;

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] ProcessPaymentCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Thanh toán thành công!" });
    }
}