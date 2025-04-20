using Common.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace OrderAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController(OrderService.OrderService orderService) : ControllerBase
	{
		[HttpPost(nameof(Create))]
		public async Task<IActionResult> Create(CreateOrderRequestDto requestDto)
		{
			var result = await orderService.CreateAsync(requestDto);
			return new ObjectResult(result) { StatusCode = result.StatusCode };
		}
	}
}
