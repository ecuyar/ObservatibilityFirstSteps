using StackExchange.Redis;

namespace OrderAPI.RedisServices
{
	public sealed class RedisService(IConnectionMultiplexer connectionMultiplexer, int dbIndex)
	{
		private readonly IDatabase _database = connectionMultiplexer.GetDatabase(dbIndex);

		public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
			=> await _database.StringSetAsync(key, value, expiry);

		public async Task<string?> GetAsync(string key)
			=> await _database.StringGetAsync(key);
	}
}
