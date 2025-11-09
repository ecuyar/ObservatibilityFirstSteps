namespace Logging.Shared;

public sealed class ElasticsearchSettings
{
	public required string BaseUrl { get; set; }
	public required string Username { get; set; }
	public required string Password { get; set; }
	public required string IndexName { get; set; }
}
