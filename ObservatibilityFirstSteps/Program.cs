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
	//.AddZipkinExporter() //use only one environment. we have two just for demo.
	.Build();

//New activity to listen only from CustomActivitySourceName source.
//Custom start and stopped messages. Writing to a file or another db can be done here instead of console messages.
ActivitySource.AddActivityListener(new ActivityListener
{
	ActivityStarted = activity =>
	{
		Console.WriteLine("Custom activity started.");
	},
	ActivityStopped = activity =>
	{
		Console.WriteLine($"Custom activity finished. Total duration:{activity.Duration}");
	},
	ShouldListenTo = source => source.Name == OpenTelemetryConstants.CustomActivitySourceName
});

//New trace provider for custom activity.
using var customTraceProvider = Sdk.CreateTracerProviderBuilder()
	.AddSource(OpenTelemetryConstants.CustomActivitySourceName)
	.Build();

//global http object
var httpClient = new HttpClient();

//run dummyJson service
var service = new DummyJsonService(httpClient);
await service.ParentGetUsers();

//run custom activity
await service.MethodForCustomListener();