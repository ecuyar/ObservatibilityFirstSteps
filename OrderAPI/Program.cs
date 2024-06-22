using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderAPI.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenTelemetryConsts>(builder.Configuration.GetSection("OpenTelemetry")); //register config data for OpenTelemetry
var openTelemetryConsts = builder.Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConsts>();

//add OpenTelemetry service
builder.Services.AddOpenTelemetry().WithTracing(config =>
{
	config.AddSource(openTelemetryConsts!.ActivitySourceName)
	.ConfigureResource(resource =>
	{
		resource.AddService(serviceName: openTelemetryConsts.ServiceName,
							serviceVersion: openTelemetryConsts.ServiceVersion);
	});

	config.AddAspNetCoreInstrumentation(); //add instrumentation
	config.AddConsoleExporter(); //add where to export data
	config.AddOtlpExporter(); //add where to export data (Jaeger)
});

//init ActivitySourceProvider
ActivitySourceProvider.Source = new(
	name: openTelemetryConsts!.ActivitySourceName,
	version: openTelemetryConsts.ServiceVersion
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
