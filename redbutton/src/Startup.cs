using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using redbutton.Middleware;

namespace redbutton
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
			=> services.AddHttpContextAccessor().AddMvcCore(options => options.EnableEndpointRouting = false);

		public void Configure(IApplicationBuilder app)
		{
			Init();

			app
				.UseLogging()
				.UseSecurityHeaders()
				.UseStatusCodePages("text/plain", "{0}")
				.Map(Settings.PathBase, app => app
					.UseDefaultFiles()
					.UseStaticFiles(StaticFileOptions)
					.UseAuth()
					.UseMvc());
		}

		private void Init()
		{
		}

		private static readonly StaticFileOptions StaticFileOptions = new StaticFileOptions
		{
			ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{".txt", "text/plain; charset=utf-8"},

				{".htm", "text/html; charset=utf-8"},
				{".html", "text/html; charset=utf-8"},

				{".json", "application/json; charset=utf-8"},

				{".css", "text/css; charset=utf-8"},
				{".js", "application/javascript; charset=utf-8"},

				{".svg", "image/svg+xml"},
				{".gif", "image/gif"},
				{".png", "image/png"},
				{".jpg", "image/jpeg"},
				{".jpeg", "image/jpeg"},
				{".webp", "image/webp"},

				{".eot", "application/vnd.ms-fontobject"},
				{".woff", "application/font-woff"},
				{".woff2", "application/font-woff2"},

				{".ico", "image/x-icon"}
			}),
			FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
			ServeUnknownFileTypes = false,
			OnPrepareResponse = ctx => ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=2419200"
		};
	}
}
