using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Shared
{
	public static class OpenTelemetryExtensions
	{
		private const string OPEN_TELEMETRY = "OpenTelemetry";

		public static void AddOpenTelemetryExt(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<OpenTelemetryConstants>(configuration.GetSection(OPEN_TELEMETRY)); //register config data for OpenTelemetry
			var openTelemetryConstants = configuration.GetSection(OPEN_TELEMETRY).Get<OpenTelemetryConstants>();

			//init ActivitySourceProvider
			ActivitySourceProvider.Source = new(
				name: openTelemetryConstants!.ActivitySourceName,
				version: openTelemetryConstants!.ServiceVersion);

			//add OpenTelemetry service
			services.AddOpenTelemetry().WithTracing(config =>
			{
				config.AddSource(openTelemetryConstants!.ActivitySourceName)
					.ConfigureResource(resource =>
					{
						resource.AddService(
							serviceName: openTelemetryConstants.ServiceName,
							serviceVersion: openTelemetryConstants.ServiceVersion);
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

				//add EF Core instrumentation
				config.AddEntityFrameworkCoreInstrumentation(options =>
				{
					options.SetDbStatementForText = true;
					options.SetDbStatementForStoredProcedure = true;
					options.EnrichWithIDbCommand = (activity, dbCommand) =>
					{
						//we can add extra info for our db script logs
					};
				});

				config.AddConsoleExporter(); //add where to export data
				config.AddOtlpExporter(); //add where to export data (Jaeger)
			});
		}
	}
}
