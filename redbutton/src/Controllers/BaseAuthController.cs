using Microsoft.AspNetCore.Mvc;
using redbutton.Middleware;

namespace redbutton.Controllers
{
	public class BaseAuthController : Controller
	{
		public new string User => Request.HttpContext.Items[AuthMiddleware.UserContextItemName] as string;
		public string Team => Request.HttpContext.Items[AuthMiddleware.TeamContextItemName] as string;

		public bool IsAdmin => /*User == "root" && */Team == "root";
		public bool IsAuthenticated => /*!string.IsNullOrEmpty(User) && */!string.IsNullOrEmpty(Team);
	}
}
