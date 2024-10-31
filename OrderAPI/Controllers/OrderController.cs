using Microsoft.AspNetCore.Mvc;
using OrderAPI.OrderService;

namespace OrderAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController(OrderService.OrderService orderService) : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> Create(CreateOrderRequestDto requestDto)
		{
			return Ok(await orderService.CreateAsync(requestDto));
		}
	}
}
