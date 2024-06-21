namespace ObservatibilityFirstSteps
{
	public class DummyJsonService
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

			try
			{
				var eventTags = new ActivityTagsCollection();

				activity?.AddEvent(new ActivityEvent("API call started.", tags: eventTags));
				activity?.AddTag("request.schema", "https"); //activity tags
				activity?.AddTag("request.method", "get");

				var httpResponse = await _httpClient.GetAsync($"{_baseUrl}/users");
				var response = await httpResponse.Content.ReadAsStringAsync();

				eventTags.Add("response.length", response.Length); //event tag
				activity?.AddEvent(new ActivityEvent("API call finished.", tags: eventTags));

				Console.WriteLine($"{nameof(GetUsers)} Status Code: {httpResponse.StatusCode}");
				Console.WriteLine($"{nameof(GetUsers)} Return Size: {response.Length} bytes. {Convert.ToDouble(response.Length) / 1_000_000} MB.");
			}
			catch (Exception ex)
			{
				//set status only in errors
				activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			}

		}

		public async Task ParentGetUsers()
		{
			using var activity = ActivitySourceProvider.Source.StartActivity(kind: ActivityKind.Producer, name: nameof(ParentGetUsers));

			await GetUsers();
		}

		public async Task MethodForCustomListener()
		{
			using var activity = ActivitySourceProvider.CustomSource.StartActivity();

			activity?.AddEvent(new ActivityEvent("Custom activity event."));
			activity?.AddTag("CustomActivityTag", "CustomActivityTagValue");
			activity?.AddTag("CustomActivityTag2", "CustomActivityTagValue2");

			Task.Delay(1000).Wait();
			await Task.FromResult(99);
		}
	}
}
