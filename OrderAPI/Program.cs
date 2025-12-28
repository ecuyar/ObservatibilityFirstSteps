using Common.Shared.Middlewares;
using Logging.Shared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using OpenTelemetry.Shared;
using OpenTelemetry.Shared.Middlewares;
using OrderAPI.Context;
using OrderAPI.OrderService;
using OrderAPI.RedisServices;
using OrderAPI.StockServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetryExt(builder.Configuration);
builder.Services.AddSingleton<RecyclableMemoryStreamManager>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddOptions<RedisSettings>()
	.Bind(builder.Configuration.GetSection("Redis"))
	.ValidateDataAnnotations()
	.ValidateOnStart();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
	var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
	return ConnectionMultiplexer.Connect(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
	var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
	var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
	return new RedisService(multiplexer, redisSettings.Database);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("TestDb"));
});

builder.Services.AddHttpClient<StockService>(options =>
{
	options.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices:StockApi").Value!);
});

builder.Services.AddMassTransit(x =>
{
	x.UsingRabbitMq((context, config) =>
	{
		config.Host("localhost", "/", host =>
		{
			host.Username("guest");
			host.Password("guest");
		});
	});
});

//builder.Host.UseSerilog(Logging.Shared.Logging.ConfigureLogging);
builder.AddOpenTelemetryLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//built-in middleware
app.UseHttpsRedirection();

//custom middleware
app.UseOpenTelemetryTraceIdMiddleware();
app.UseReadResponseMiddleware();
app.UseExceptionMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
