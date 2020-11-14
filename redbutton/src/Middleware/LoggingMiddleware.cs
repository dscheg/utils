using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using redbutton.Controllers;

namespace redbutton.Middleware
{
	public class LoggingMiddleware
	{
		public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
		{
			this.next = next;
			this.logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			context.Items["round"] = MainController.CurrentRound.Rnd;
			await next.Invoke(context).ConfigureAwait(false);
		}

		private readonly RequestDelegate next;
		private readonly ILogger<LoggingMiddleware> logger;
	}

	public static class LogContextMiddlewareExtensions
	{
		public static IApplicationBuilder UseLogging(this IApplicationBuilder builder)
			=> builder.UseMiddleware<LoggingMiddleware>();
	}
}
