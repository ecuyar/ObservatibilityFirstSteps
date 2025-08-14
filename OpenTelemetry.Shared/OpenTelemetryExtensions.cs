using MassTransit.Logging;
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
				config
				.AddSource(openTelemetryConstants!.ActivitySourceName)
				.AddSource(DiagnosticHeaders.DefaultListenerName) //watch masstransit traces
				.ConfigureResource(resource =>
				{
					resource.AddService(
						serviceName: openTelemetryConstants.ServiceName,
						serviceVersion: openTelemetryConstants.ServiceVersion);
				});

				//add asp net instrumentation
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

				//add http instrumentation
				config.AddHttpClientInstrumentation(options =>
				{
					//These options can be commented because services have middlewares that log request/response bodies.
					//Can be used for external service call logs.
					options.EnrichWithHttpRequestMessage = async (activity, request) =>
					{
						var requestContent = string.Empty;

						//GET requests' body will be null, so we need to control it
						if (request.Content is not null)
							requestContent = await request.Content.ReadAsStringAsync();

						activity.SetTag("http.request.body", requestContent);
					};

					options.EnrichWithHttpResponseMessage = async (activity, response) =>
					{
						if (response.Content is not null)
							activity.SetTag("http.response.body", await response.Content.ReadAsStringAsync());
					};
				});

				//add redis instrumentation
				config.AddRedisInstrumentation(option =>
				{
					option.SetVerboseDatabaseStatements = true;
				});

				config.AddConsoleExporter(); //add where to export data
				config.AddOtlpExporter(); //add where to export data (Jaeger)
			});
		}
	}
}
