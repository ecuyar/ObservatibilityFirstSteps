using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace Logging.Shared;

public static class ElasticsearchLogging
{
	public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging => (builderContext, loggerConfiguration) =>
	{
		var environment = builderContext.HostingEnvironment;

		loggerConfiguration
		.ReadFrom.Configuration(builderContext.Configuration)
		.Enrich.FromLogContext()
		.Enrich.WithExceptionDetails()
		.Enrich.WithProperty("Env", environment.EnvironmentName)
		.Enrich.WithProperty("AppName", environment.ApplicationName);

		var elasticsearchBaseUrl = builderContext.Configuration.GetSection("Elasticsearch")["BaseUrl"] ?? throw new ArgumentException();
		var username = builderContext.Configuration.GetSection("Elasticsearch")["Username"] ?? throw new ArgumentException();
		var password = builderContext.Configuration.GetSection("Elasticsearch")["Password"] ?? throw new ArgumentException();
		var indexName = builderContext.Configuration.GetSection("Elasticsearch")["IndexName"] ?? throw new ArgumentException();

		loggerConfiguration.WriteTo.Elasticsearch([new Uri(elasticsearchBaseUrl)], options =>
		{
			options.DataStream = new DataStreamName("logs", indexName, environment.EnvironmentName); //this naming will create a dataStream like logs-order.api-production
			options.BootstrapMethod = BootstrapMethod.Failure;
			options.ConfigureChannel = channelOpts =>
			{
				channelOpts.BufferOptions = new BufferOptions
				{
					ExportMaxConcurrency = 10
				};
			};
		}, transport =>
		{
			transport.Authentication(new BasicAuthentication(username, password));
		});
	};
}
