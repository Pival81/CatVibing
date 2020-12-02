using System;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.Controllers;
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
			services.AddControllers()
				.AddXmlSerializerFormatters()
				.AddNewtonsoftJson();
			FleckServer.Start(socket =>
			{
				var cancToken = new CancellationTokenSource();
				var Task = new Task(() =>
				{
					var id = socket.ConnectionInfo.Path.Substring(1);
					Meme resource = null;
					try { resource = MemeGenerator.MemeRepo.Get(new Guid(id)); }
					catch (Exception e) {}
					if (resource == null)
					{
						socket.Send("ERROR");
						socket.Close();
						return;
					}
					bool completed = false;
					int num = -1;
					while (!completed)
					{
						if (resource.WorkStatus.Percentage == num)
							continue;
						if (resource.WorkStatus.Status == WorkStatus.Done)
							completed = true;
						num = resource.WorkStatus.Percentage;
						String response = $"{num:D2}\n";
						socket.Send(response);
						if(resource.WorkStatus.Percentage == 100)
							socket.Send("DONE\n");
					}
					socket.Close();
				}, cancToken.Token);
				socket.OnOpen = () => Task.Start();
				socket.OnClose = () => cancToken.Cancel();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationLifetime applicationLifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			applicationLifetime.ApplicationStopping.Register(() => FleckServer.Dispose());

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseWebSockets();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}