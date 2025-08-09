using Common.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace StockAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StockController(StockService.StockService stockService) : ControllerBase
	{
		[HttpPost(nameof(CheckAndStartPayment))]
		public async Task<IActionResult> CheckAndStartPayment(CheckAndPaymentServiceRequestDto requestDto)
		{
			var result = await stockService.CheckAndPaymentService(requestDto);
			return new ObjectResult(result) { StatusCode = result.StatusCode };
		}
	}
}
