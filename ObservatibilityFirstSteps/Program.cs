//create OpenTelemetry service
//no data exporting without using block
using var traceProvider = Sdk.CreateTracerProviderBuilder()
	.AddSource(OpenTelemetryConstants.ActivitySourceName)
	.ConfigureResource(config =>
	{
		config
		.AddService(
			serviceName: OpenTelemetryConstants.ServiceName,
			serviceVersion: OpenTelemetryConstants.ServiceVersion)
		.AddAttributes(new List<KeyValuePair<string, object>>()
		{
			new("host.machineName", Environment.MachineName),
			new("host.environment", "dev")
		});
	})
	.AddConsoleExporter()
	.AddOtlpExporter()
	.Build();

//global http object
var httpClient = new HttpClient();

//run dummyJson service
var service = new DummyJsonService(httpClient);
await service.ParentGetUsers();
