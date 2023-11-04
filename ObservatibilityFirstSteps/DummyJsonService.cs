namespace ObservatibilityFirstSteps
{
	internal class DummyJsonService
	{
		private readonly HttpClient _httpClient;
		private readonly string _baseUrl = "https://dummyjson.com";

		// This class has two methods in it. In trace data, there will be two SpanId but only one TraceId.
		// GetUsers trace data will have ParentSpanId parameter.
		public DummyJsonService(HttpClient client)
		{
			_httpClient = client;
		}

		private async Task GetUsers()
		{
			using var activity = ActivitySourceProvider.Source.StartActivity();

			var httpResponse = await _httpClient.GetAsync($"{_baseUrl}/users");
			var response = await httpResponse.Content.ReadAsStringAsync();

			Console.WriteLine($"{nameof(GetUsers)} Status Code: {httpResponse.StatusCode}");
			Console.WriteLine($"{nameof(GetUsers)} Return Size: {response.Length} bytes. {Convert.ToDouble(response.Length) / 1_000_000} MB.");
		}

		internal async Task ParentGetUsers()
		{
			using var activity = ActivitySourceProvider.Source.StartActivity(kind: ActivityKind.Producer, name: nameof(ParentGetUsers));

			await GetUsers();
		}
	}
}
