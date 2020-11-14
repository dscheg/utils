using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using redbutton.Middleware;

namespace redbutton.Controllers
{
	[Route("api")]
	[ApiController]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public class MainController : BaseAuthController
	{
		public MainController(ILogger<LoggingMiddleware> logger)
			=> this.logger = logger;

		[HttpGet("result")]
		public IActionResult GetResult()
		{
			var round = CurrentRound;

			var ticks = round.ElapsedTicks;
			logger.LogInformation($"round #{round.Rnd:x8}, elapsed {(ticks / 10000 / 1000.0).ToString("0.000", NumberFormatInfo.InvariantInfo)} sec, count {round.Order.Count}");

			return Ok(new
			{
				rand = round.Rnd,
				time = ticks,
				list = round.Order.Where(pair => pair.Value.idx != 0).OrderBy(pair => pair.Value.idx).Select((pair, idx) => new {pair.Value.idx, team = pair.Key, pair.Value.time})
			});
		}

		[HttpPost("push")]
		public IActionResult Push()
		{
			if(!IsAuthenticated)
				return StatusCode(403, "forbidden");

			if(!CurrentRound.IsStarted)
				return StatusCode(425, "wait for start");

			var (idx, ticks) = CurrentRound.GetOrSet(Team);

			var result = $"you are #{idx} in {(ticks / 10000 / 1000.0).ToString("0.000", NumberFormatInfo.InvariantInfo)} sec";
			logger.LogInformation(result);

			return Ok(result);
		}

		[HttpPost("restart")]
		public IActionResult Restart()
		{
			if(!IsAdmin)
				return StatusCode(403, "forbidden");

			CurrentRound = Round.StartNew();

			const string result = "restarted";
			logger.LogInformation(result);

			return Ok(result);
		}

		[HttpPost("stop")]
		public IActionResult Stop()
		{
			if(!IsAdmin)
				return StatusCode(403, "forbidden");

			CurrentRound.Stop();

			const string result = "stopped";
			logger.LogInformation(result);

			return Ok("stopped");
		}

		[HttpPost("start")]
		public IActionResult Start()
		{
			if(!IsAdmin)
				return StatusCode(403, "forbidden");

			CurrentRound.Start();

			const string result = "started";
			logger.LogInformation(result);

			return Ok(result);
		}

		[HttpPost("clear")]
		public IActionResult Clear()
		{
			if(!IsAdmin)
				return StatusCode(403, "forbidden");

			CurrentRound = new Round();

			const string result = "cleared";
			logger.LogInformation(result);

			return Ok(result);
		}

		public static volatile Round CurrentRound = new Round();

		private readonly ILogger<LoggingMiddleware> logger;
	}
}
