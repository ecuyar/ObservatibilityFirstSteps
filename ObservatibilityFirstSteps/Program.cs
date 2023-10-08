Console.WriteLine("Hello, World!");

//create OpenTelemetry service
var traceProvider = Sdk.CreateTracerProviderBuilder()
	.AddSource(OpenTelemetryConstants.ActivitySourceName)
	.ConfigureResource(config =>
	{
		config
		.AddService(OpenTelemetryConstants.ServiceName, OpenTelemetryConstants.ServiceVersion)
		.AddAttributes(new List<KeyValuePair<string, object>>()
		{
			new KeyValuePair<string, object>("host.machineName", Environment.MachineName),
			 new KeyValuePair<string, object>("host.environment", "dev")
		});
	}).Build();
