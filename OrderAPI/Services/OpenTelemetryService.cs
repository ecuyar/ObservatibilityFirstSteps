using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderAPI.OpenTelemetry;

namespace OrderAPI.Services
{
	public static class OpenTelemetryService
	{
		private const string OPEN_TELEMETRY = "OpenTelemetry";

		public static IServiceCollection AddOpenTelemetyService(this IServiceCollection services, IConfiguration configuration)
		{

			services.Configure<OpenTelemetryConsts>(configuration.GetSection(OPEN_TELEMETRY)); //register config data for OpenTelemetry
			var openTelemetryConsts = configuration.GetSection(OPEN_TELEMETRY).Get<OpenTelemetryConsts>();

			//add OpenTelemetry service
			services.AddOpenTelemetry().WithTracing(config =>
			{
				config.AddSource(openTelemetryConsts!.ActivitySourceName)
					.ConfigureResource(resource =>
					{
						resource.AddService(
							serviceName: openTelemetryConsts.ServiceName,
							serviceVersion: openTelemetryConsts.ServiceVersion);
					});

				//add instrumentation
				config.AddAspNetCoreInstrumentation(options =>
				{
					options.Filter = context =>
					{
						//filter only controller based requests' trace data, not system based requests
						var pathValue = context.Request.Path.Value;
						return pathValue is not null && pathValue.Contains("api", StringComparison.InvariantCultureIgnoreCase);
					};
					options.RecordException = true;  //get detailed exception instead of just Exception.Message
				});
				config.AddConsoleExporter(); //add where to export data
				config.AddOtlpExporter(); //add where to export data (Jaeger)
			});

			//init ActivitySourceProvider
			ActivitySourceProvider.Source = new(
				name: openTelemetryConsts!.ActivitySourceName,
				version: openTelemetryConsts!.ServiceVersion
				);

			return services;
		}
	}
}
