using Common.Shared.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Shared.Middlewares;

public static class ExceptionMiddleware
{
	public static void UseExceptionMiddleware(this WebApplication app)
	{
		app.UseExceptionHandler(config =>
		{
			config.Run(async context =>
			{
				var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
				var exception = exceptionFeature?.Error;

				var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ExceptionMiddleware));
				logger.LogError(exception, "Unhandled exception occurred");

				context.Response.StatusCode = 400;

				var response = ResponseDto<string>.Fail(400, exception?.Message ?? "An unexpected error occurred. Please try again later.");
				await context.Response.WriteAsJsonAsync(response);
			});
		});
	}
}
