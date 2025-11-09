using Common.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace PaymentAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentProcessController(ILogger<PaymentProcessController> logger) : ControllerBase
	{
		[HttpPost("create")]
		public IActionResult Create(PaymentCreateRequestDto request)
		{
			const decimal balance = 1000;

			if (request.TotalPrice > balance)
			{
				logger.LogInformation("Yetersiz bakiye. {@orderCode}", request.OrderCode);
				return BadRequest(ResponseDto<PaymentCreateResponseDto>.Fail(400, "Yetersiz bakiye"));
			}

			logger.LogInformation("Kart iþlemi baþarýyla gerçekleþmiþtir. {@orderCode} {@totalPrice}", request.OrderCode, request.TotalPrice);
			return Ok(ResponseDto<PaymentCreateResponseDto>.Success(
				200,
				new PaymentCreateResponseDto
				{
					Description = "Kart iþlemi baþarýyla gerçekleþmiþtir."
				}));
		}
	}
}
