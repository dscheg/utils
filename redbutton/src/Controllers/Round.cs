using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

namespace redbutton.Controllers
{
	public class Round
	{
		public static Round StartNew()
		{
			var round = new Round();
			round.Start();
			return round;
		}

		public void Start() => stopwatch.Start();
		public void Stop() => stopwatch.Stop();
		public bool IsStarted => stopwatch.IsRunning;
		public long ElapsedTicks => stopwatch.ElapsedTicks;

		public (int idx, long ticks) GetOrSet(string team)
		{
			//NOTE: The order of the indices may not be the same as the order of the ticks!
			var (idx, ticks) = Order.GetOrAdd(team, _ => (0, stopwatch.ElapsedTicks));
			if(idx == 0) Order[team] = (idx = Interlocked.Increment(ref index), ticks);
			return (idx, ticks);
		}

		public readonly ConcurrentDictionary<string, (int idx, long time)> Order = new ConcurrentDictionary<string, (int, long)>();
		public readonly int Rnd = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		public readonly Stopwatch stopwatch = new Stopwatch();
		public int index;
	}
}