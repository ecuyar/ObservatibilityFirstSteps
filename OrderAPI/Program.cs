using Microsoft.EntityFrameworkCore;
using Microsoft.IO;
using OpenTelemetry.Shared;
using OrderAPI.Context;
using OrderAPI.Middlewares;
using OrderAPI.OrderService;
using OrderAPI.StockServices;

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

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("TestDb"));
});

builder.Services.AddHttpClient<StockService>(options =>
{
	options.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices:StockApi").Value!);
});

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
app.UseReadResponseMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
