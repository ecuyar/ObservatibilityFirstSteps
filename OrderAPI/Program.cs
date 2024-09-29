using Microsoft.IO;
using OrderAPI.Middlewares;
using OrderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetyService(builder.Configuration);
builder.Services.AddSingleton<RecyclableMemoryStreamManager>();

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
