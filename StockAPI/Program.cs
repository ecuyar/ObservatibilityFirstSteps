using Microsoft.IO;
using OpenTelemetry.Shared;
using OpenTelemetry.Shared.Middlewares;
using StockAPI.StockService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<StockService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetryExt(builder.Configuration);
builder.Services.AddSingleton<RecyclableMemoryStreamManager>();

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
