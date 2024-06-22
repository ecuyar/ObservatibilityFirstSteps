namespace OrderAPI.OpenTelemetry
{
	public class OpenTelemetryConsts
	{
		public string ServiceName { get; set; } = null!;
		public string ServiceVersion { get; set; } = null!;
		public string ActivitySourceName { get; set; } = null!;
	}
}