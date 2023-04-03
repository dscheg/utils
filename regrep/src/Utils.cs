using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class Utils
{
	internal static IEnumerable<string> ReadLines(this TextReader reader)
	{
		string line;
		while((line = reader.ReadLine()) != null)
			yield return line;
	}

	internal static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		foreach(var item in enumerable)
			action(item);
	}

	internal static ParallelQuery<T> With<T>(this ParallelQuery<T> enumerable, Action<T> action) => enumerable.Select(item =>
	{
		action(item);
		return item;
	});
}
