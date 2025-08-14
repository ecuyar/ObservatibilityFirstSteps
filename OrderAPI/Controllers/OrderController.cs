using Common.Shared.Dtos;
using Common.Shared.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace OrderAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController(
		OrderService.OrderService orderService,
		IPublishEndpoint publishEndpoint) : ControllerBase
	{
		[HttpPost(nameof(Create))]
		public async Task<IActionResult> Create(CreateOrderRequestDto requestDto)
		{
			var result = await orderService.CreateAsync(requestDto);
			return new ObjectResult(result) { StatusCode = result.StatusCode };
		}

		[HttpPost(nameof(SendOrderCreatedEvent))]
		public async Task<IActionResult> SendOrderCreatedEvent()
		{
			await publishEndpoint.Publish(new OrderCreatedEvent { OrderCode = Guid.NewGuid().ToString() });
			return Ok();
		}
	}
}
