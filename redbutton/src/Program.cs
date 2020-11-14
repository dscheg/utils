using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;

namespace redbutton
{
	public class Program
	{
		public static void Main()
		{
			NLog.Config.ConfigurationItemFactory.Default = new NLog.Config.ConfigurationItemFactory(typeof(NLog.ILogger).GetTypeInfo().Assembly);
			LayoutRenderer.Register<Int32HexLayoutRenderer>("int32hex");
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
			CreateWebHostBuilder().Build().Run();
			NLog.LogManager.Flush();
			NLog.LogManager.Shutdown();
		}

		private static IWebHostBuilder CreateWebHostBuilder() => new WebHostBuilder()
			.UseConfigurationSection(Settings.ConfigRoot.GetSection("Host"))
			.ConfigureLogging((host, logging) => logging.AddConfiguration(host.Configuration.GetSection("Logging")).AddNLog("settings/nlog.config"))
			.UseNLog()
			.UseKestrel((host, options) =>
			{
				options.AddServerHeader = false;
				options.Limits.MaxRequestBodySize = 0L;
				options.Limits.MaxRequestHeaderCount = 64;
				options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(3);
			})
			.UseStartup<Startup>();
	}

	public static class WebHostConfigurationSection
	{
		public static IWebHostBuilder UseConfigurationSection(this IWebHostBuilder builder, IConfiguration configuration)
		{
			foreach(var setting in configuration.AsEnumerable(true))
				builder.UseSetting(setting.Key, setting.Value);
			return builder;
		}
	}
}
