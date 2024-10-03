using Microsoft.IO;
using System.Diagnostics;

namespace OrderAPI.Middlewares
{
	public class RequestResponseMiddleware(RequestDelegate next, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
	{
		private readonly RequestDelegate _next = next;
		private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = recyclableMemoryStreamManager;

		public async Task Invoke(HttpContext httpContext)
		{
			await RequestBodyObserver(httpContext);
			await ResponseBodyObserver(httpContext);
		}

		//marked static because of CA1822 warning. (Method doesn't use any instance data)
		private static async Task RequestBodyObserver(HttpContext httpContext)
		{
			//request can be read multiple times so buffer it
			httpContext.Request.EnableBuffering();

			using var streamReader = new StreamReader(httpContext.Request.Body);
			var requestBody = await streamReader.ReadToEndAsync();

			//save body with a proper tag
			Activity.Current?.SetTag("http.request.body", requestBody);
			//reset stream reading head for other readings
			httpContext.Request.Body.Position = 0;
		}

		private async Task ResponseBodyObserver(HttpContext httpContext)
		{
			var originalResponse = httpContext.Response.Body;

			await using var memoryStream = _recyclableMemoryStreamManager.GetStream();
			//give body a new stream to read
			httpContext.Response.Body = memoryStream;

			//let other middlewares continue
			await _next(httpContext);

			memoryStream.Position = 0;
			var streamReader = new StreamReader(memoryStream);
			var body = await streamReader.ReadToEndAsync();

			//save body with a proper tag
			Activity.Current?.SetTag("http.response.body", body);
			//reset reader head
			memoryStream.Position = 0;
			await memoryStream.CopyToAsync(originalResponse);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ReadResponseMiddlewareExtensions
	{
		public static IApplicationBuilder UseReadResponseMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<RequestResponseMiddleware>();
		}
	}
}
