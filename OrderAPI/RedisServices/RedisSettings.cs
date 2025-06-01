using System.ComponentModel.DataAnnotations;

namespace OrderAPI.RedisServices
{
	public sealed class RedisSettings
	{
		[Range(0, 15)]
		public int Database { get; set; }

		[Required]
		public string ConnectionString { get; set; } = string.Empty;
	}
}
