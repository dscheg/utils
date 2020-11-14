using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace redbutton
{
	public abstract class Settings
	{
		static Settings()
		{
			ConfigRoot = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddIniFile(Path.Combine(Directory.GetCurrentDirectory(), SettingsFilePath), false, true)
				.Build();

			ChangeToken.OnChange(() => ConfigRoot.GetReloadToken(), () =>
			{
				try
				{
					Update();
				}
				catch(Exception e)
				{
					Console.Error.WriteLine($"Failed to reload settings '{SettingsFilePath}'", e);
				}
			});

			ConfigRoot.Reload();
		}

		public static readonly IConfigurationRoot ConfigRoot;

		public static string PathBase { get; private set; }

		private static void Update()
		{
			var config = ConfigRoot.GetSection("RedButton");
			PathBase = config["PathBase"];
		}

		private const string SettingsFilePath = "settings/redbutton.ini";
	}
}
