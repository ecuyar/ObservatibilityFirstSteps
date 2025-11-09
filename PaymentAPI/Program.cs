using Logging.Shared;
using Microsoft.IO;
using OpenTelemetry.Shared;
using OpenTelemetry.Shared.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(ElasticsearchLogging.ConfigureLogging);

// Add services to the container.

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

//custom middlewares
app.UseOpenTelemetryTraceIdMiddleware();
app.UseReadResponseMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
