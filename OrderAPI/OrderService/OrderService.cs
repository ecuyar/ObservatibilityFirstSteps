using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Shared;
using System.Diagnostics;

namespace OrderAPI.OrderService
{
	public class OrderService(DbContext context)
	{
		public async Task<CreateOrderResponseDto> CreateAsync(CreateOrderRequestDto requestDto)
		{
			Activity.Current?.SetTag("Asp.Net Core tag1", "Asp.Net Core tag value");
			using var activity = ActivitySourceProvider.Source.StartActivity();
			activity?.AddEvent(new("Sipariş süreci başladı."));

			var newOrder = new Order()
			{
				CreatedAt = DateTime.Now,
				CreatedBy = requestDto.UserId,
				Items = requestDto.Items.Select(x => new OrderItem
				{
					Count = x.Count,
					ProductId = x.ProductId,
					UnitPrice = x.UnitPrice
				}).ToList(),
				OrderCode = Guid.NewGuid().ToString(),
			};

			context.Add(newOrder);
			await context.SaveChangesAsync();

			activity?.SetTag("Order user id", requestDto.UserId);
			activity?.AddEvent(new("Sipairş süreci tamamlandı."));

			return new CreateOrderResponseDto { Id = newOrder.Id };
		}
	}
}
