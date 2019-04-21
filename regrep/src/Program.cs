using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using NDesk.Options;

namespace regrep
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				int threads = 1;
				Encoding encoding = Encoding.UTF8;
				string pattern = null, replacement = null;
				bool invertMatch = false, ignoreCase = false, onlyMatching = false, silent = !(Console.IsInputRedirected && Console.IsOutputRedirected), showHelpAndExit = false;

				var options = new OptionSet
				{
					{"e|encoding=", "Input/output {ENCODING} (default UTF-8)", v =>
					{
						try
						{
							encoding = int.TryParse(v, out var codepage)
								? Encoding.GetEncoding(codepage)
								: Encoding.GetEncoding(v);
						}
						catch
						{
							throw new OptionException("Not supported encoding", "encoding");
						}
					}},
					{"i|ignore-case", "Ignore case distinctions", v => ignoreCase = v != null},
					{"v|invert-match", "Select non matching lines", v => invertMatch = v != null},
					{"o|only-matching", "Show only the part of a line matching PATTERN", v => onlyMatching = v != null},
					{"n|threads=", "Number of threads (default 1)", (int v) => threads = Math.Min(Math.Max(1, v), Environment.ProcessorCount)},
					{"s|silent", "Silent mode", v => silent = v != null},
					{"h|?|help", "Show this message", v => showHelpAndExit = v != null}
				};

				List<string> rest = null;
				try
				{
					rest = options.Parse(args);
				}
				catch(OptionException e)
				{
					Console.Error.WriteLine("Option '{0}' value is invalid: {1}", e.OptionName, e.Message);
					Console.Error.WriteLine();
					showHelpAndExit = true;
				}

				if(rest?.Count > 0)
				{
					pattern = rest[0];
					if(rest.Count > 1)
						replacement = rest[1];
				}

				if(invertMatch && onlyMatching)
				{
					Console.Error.WriteLine("Only-matching & invert-match can't be used together");
					showHelpAndExit = true;
				}

				if(showHelpAndExit || pattern == null)
				{
					Console.Error.WriteLine(@"
Usage: regrep [OPTIONS] PATTERN [REPLACEMENT]

Search and replace for PATTERN in standard input using regular expression.

Options:");
					options.WriteOptionDescriptions(Console.Error);
					Console.Error.WriteLine(@"
Examples:
  regrep \d
  regrep \s+ "" ""
  regrep -e1251 [Пп]ривет
  regrep -eUTF-16LE \x22\p{Lu}+\x22
  regrep -o ""\b(\d{2})\.(\d{2})\.(\d{4})\b"" ""$3-$2-$1\t$'""");
					Console.Error.WriteLine(@"
Substitutions:
  $$       Single $ literal
  $NUMBER  Last substring matched by the capturing group identified by NUMBER
  ${NAME}  Last substring matched by the named group identified by (?<NAME>)
  $&       Copy of the entire match
  $`       Text of the input string before the match
  $'       Text of the input string after the match
  $+       Last group captured
  $_       Entire input string

Supports character escapes in both PATTERN and REPLACEMENT:
  \X       Well-known \a, \b, \e, \t, \r, \n, \v, \f
  \cX      ASCII control char, X is the letter of the control char
  \NNN     Two or three digits octal character code
  \xNN     Two-digit hexadecimal character code
  \uNNNN   UTF-16 hexadecimal code unit

Exit status is 0 if any line is selected, 1 otherwise, 2 if any error occurs.");
					Console.Error.WriteLine();
					Environment.Exit(2);
				}

				var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));

				Console.InputEncoding = encoding;
				Console.OutputEncoding = encoding;

				long lines = 0L, matches = 0L;
				var total = new Stopwatch();

				var stdErrHandle = Native.GetStdHandle(Native.STD_ERROR_HANDLE);
				var stdOutHandle = Native.GetStdHandle(Native.STD_OUTPUT_HANDLE);

				silent = silent || Console.IsErrorRedirected || stdErrHandle == Native.INVALID_HANDLE_VALUE || stdOutHandle == Native.INVALID_HANDLE_VALUE || Native.GetFileType(new SafeFileHandle(stdOutHandle, false)) == Native.FILE_TYPE_PIPE;

				var lockobj = new object();
				void PrintStats(object state)
				{
					if(!(total.IsRunning || state is bool force && force))
						return;

					try
					{
						lock(lockobj)
						{
							if(!Native.GetConsoleScreenBufferInfo(stdErrHandle, out var info))
								return;

							Native.SetConsoleCursorPosition(stdErrHandle, new Native.COORD {X = 0, Y = (short)(info.dwCursorPosition.Y - 2)});

							var lns = Interlocked.Read(ref lines);
							var mts = Interlocked.Read(ref matches);
							var elapsed = total.Elapsed;

							Console.Error.WriteLine("Matches: {0} / {1} lines [{2}]    ", mts, lns, (lns == 0 ? 0.0d : (double)mts / lns).ToString("0.00%", NumberFormatInfo.InvariantInfo));
							Console.Error.WriteLine("Elapsed: {0} [{1} lines/sec]      ", elapsed.ToString("h\\:mm\\:ss\\.f", CultureInfo.InvariantCulture), (elapsed.Ticks == 0 ? 0.0d : lns * 10000000.0 / elapsed.Ticks).ToString("0.0", NumberFormatInfo.InvariantInfo));
						}
					}
					catch { /* ignore */ }
				}

				if(!silent)
				{
					Console.Error.WriteLine("Pattern: /{0}/{1} -> {2}", pattern, ignoreCase ? "i" : null, replacement ?? (onlyMatching ? "$&" : "$_"));
					Console.Error.WriteLine(Environment.NewLine);
				}

				if(replacement != null)
					replacement = Regex.Unescape(replacement);

				using var tick = silent ? null : new Timer(PrintStats, false, 100, 100);

				total.Start();
				var source = Console.In.ReadLines().With(line => Interlocked.Increment(ref lines)).AsParallel().AsOrdered().WithDegreeOfParallelism(threads);
				if(onlyMatching)
				{
					var result = source.SelectMany(line => invertMatch ? Enumerable.Empty<Match>() : regex.Matches(line).OfType<Match>());
					source = replacement == null ? result.Select(m => m.Value) : result.Select(m => m.Result(replacement));
				}
				else
				{
					source = source.Where(line => invertMatch ^ regex.IsMatch(line));
					if(replacement != null) source = source.Select(line => regex.Replace(line, replacement));
				}
				source.AsSequential().With(match => Interlocked.Increment(ref matches)).ForEach(Console.WriteLine);

				total.Stop();
				tick?.Change(Timeout.Infinite, Timeout.Infinite);

				if(!silent) PrintStats(true);

				Environment.Exit(matches > 0 ? 0 : 1);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine(e.Message);
				Environment.Exit(2);
			}
		}

		private static IEnumerable<string> ReadLines(this TextReader reader)
		{
			string line;
			while((line = reader.ReadLine()) != null)
				yield return line;
		}

		private static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach(var item in enumerable)
				action(item);
		}

		private static IEnumerable<T> With<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach(var item in enumerable)
			{
				action(item);
				yield return item;
			}
		}
	}
}