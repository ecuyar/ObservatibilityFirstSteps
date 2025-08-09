using Common.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace PaymentAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentProcessController : ControllerBase
	{
		[HttpPost("create")]
		public IActionResult Create(PaymentCreateRequestDto request)
		{
			const decimal balance = 1000;

			if (request.TotalPrice > balance)
				return BadRequest(ResponseDto<PaymentCreateResponseDto>.Fail(400, "Yetersiz bakiye"));

			return Ok(ResponseDto<PaymentCreateResponseDto>.Success(
				200,
				new PaymentCreateResponseDto
				{
					Description = "Kart iþlemi baþarýyla gerçekleþmiþtir."
				}));
		}
	}
}
