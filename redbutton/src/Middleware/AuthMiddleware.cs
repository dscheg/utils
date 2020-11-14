using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace redbutton.Middleware
{
	public class AuthMiddleware
	{
		public AuthMiddleware(RequestDelegate next) => this.next = next;

		public const string UserContextItemName = "user";
		public const string TeamContextItemName = "team";

		public async Task Invoke(HttpContext context)
		{
			var user = context.Request.Cookies["user"]?.Trim();
			if(user?.Length > MaxLength) user = user.Substring(0, MaxLength);
			context.Items[UserContextItemName] = user;

			var team = context.Request.Cookies["team"]?.Trim();
			if(team?.Length > MaxLength) team = team.Substring(0, MaxLength);
			context.Items[TeamContextItemName] = team;

			await next.Invoke(context).ConfigureAwait(false);
		}

		private readonly RequestDelegate next;
		private const int MaxLength = 64;
	}

	public static class PayStatAuthExtensions
	{
		public static IApplicationBuilder UseAuth(this IApplicationBuilder builder)
			=> builder.UseMiddleware<AuthMiddleware>();
	}
}
