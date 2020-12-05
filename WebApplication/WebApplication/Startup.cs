using System;
using System.Threading;
using Fleck;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using WebApplication.Controllers;
using WebApplication.Repositories;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace WebApplication
{
	public class Startup
	{
		public static String ContentRoot;
		public static WebSocketServer FleckServer = new WebSocketServer("ws://127.0.0.1:8181");
		
		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			ContentRoot = env.ContentRootPath;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IMemeRepository, MemoryMemeRepository>();
			services.AddControllers()
				.AddXmlSerializerFormatters()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.Formatting = Formatting.Indented;
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
					options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
			IApplicationLifetime applicationLifetime, IMemeRepository memeRepository)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			
			FleckServer.Start(socket =>
			{
				var cancToken = new CancellationTokenSource();
				var Task = new FleckTask(socket, cancToken.Token, memeRepository);
				socket.OnOpen = () => Task.Start();
				socket.OnClose = () => cancToken.Cancel();
			});

			applicationLifetime.ApplicationStopping.Register(() =>
			{
				FleckServer.Dispose();
				memeRepository.Save();
			});

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}