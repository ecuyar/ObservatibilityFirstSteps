using Common.Shared.Events;
using MassTransit;
using System.Diagnostics;
using System.Text.Json;

namespace StockAPI.Consumers
{
	public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
	{
		public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
		{
			Activity.Current?.SetTag("message.body", JsonSerializer.Serialize(context.Message));

			await Task.CompletedTask;
		}
	}
}
