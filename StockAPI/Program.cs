using MassTransit;
using Microsoft.IO;
using OpenTelemetry.Shared;
using OpenTelemetry.Shared.Middlewares;
using Serilog;
using StockAPI.Consumers;
using StockAPI.PaymentServices;
using StockAPI.StockService;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Logging.Shared.ElasticsearchLogging.ConfigureLogging);

// Add services to the container.
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetryExt(builder.Configuration);
builder.Services.AddSingleton<RecyclableMemoryStreamManager>();

builder.Services.AddHttpClient<PaymentService>(options =>
{
	options.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices")["PaymentApi"]!);
});

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<OrderCreatedEventConsumer>();

	x.UsingRabbitMq((context, config) =>
	{
		config.Host("localhost", "/", host =>
		{
			host.Username("guest");
			host.Password("guest");
		});

		config.ReceiveEndpoint("stock.order-created-event.queue", e =>
		{
			e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
		});
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//custom middlewares
app.UseReadResponseMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
