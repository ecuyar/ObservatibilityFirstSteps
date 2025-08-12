using Common.Shared.Dtos;
using Common.Shared.Events;
using MassTransit;
using OpenTelemetry.Shared;
using OrderAPI.Context;
using OrderAPI.RedisServices;
using OrderAPI.StockServices;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace OrderAPI.OrderService
{
	public class OrderService(AppDbContext context, StockService stockService, RedisService redisService, IPublishEndpoint publishEndpoint)
	{
		public async Task<ResponseDto<CreateOrderResponseDto>> CreateAsync(CreateOrderRequestDto requestDto)
		{
			Activity.Current?.SetTag("Asp.Net Core tag1", "Asp.Net Core tag value");
			using var activity = ActivitySourceProvider.Source.StartActivity();
			activity?.AddEvent(new("Sipariş süreci başladı."));

			activity?.AddBaggage("userId", requestDto.UserId.ToString());

			var jsonString = JsonSerializer.Serialize(requestDto);
			await redisService.SetAsync($"{requestDto.UserId}/order", jsonString);

			var newOrder = await CreateOrderAsync(requestDto);

			//publish order creeated message
			await publishEndpoint.Publish(new OrderCreatedEvent { OrderCode = newOrder.OrderCode });

			//check stock and continue
			//TODO seperate stock checking and going to payment process
			var stockRequest = new CheckAndPaymentServiceRequestDto
			{
				OrderCode = newOrder.OrderCode,
				OrderItems = requestDto.Items
			};

			var (isSuccess, failMessage) = await stockService.CheckAndPaymentAsync(stockRequest);
			if (!isSuccess)
			{
				return ResponseDto<CreateOrderResponseDto>.Fail(HttpStatusCode.InternalServerError.GetHashCode(), failMessage ?? []);
			}

			activity?.AddEvent(new("Sipariş süreci tamamlandı."));
			return ResponseDto<CreateOrderResponseDto>.Success(HttpStatusCode.OK.GetHashCode(), new() { Id = newOrder.Id });
		}

		private async Task<Order> CreateOrderAsync(CreateOrderRequestDto requestDto)
		{
			var newOrder = new Order()
			{
				CreatedAt = DateTime.Now,
				CreatedBy = requestDto.UserId,
				Items = [.. requestDto.Items.Select(x => new OrderItem
				{
					Count = x.Count,
					ProductId = x.ProductId,
					UnitPrice = x.UnitPrice
				})],
				OrderCode = Guid.NewGuid().ToString(),
			};

			context.Add(newOrder);
			await context.SaveChangesAsync();
			return newOrder;
		}
	}
}
