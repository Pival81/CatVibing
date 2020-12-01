using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{

	[Route("meme")]
	[ApiController]
	public class MemeGenerator : ControllerBase
	{

		public static MemoryMemeRepository MemeRepo = new MemoryMemeRepository();

		[HttpPost]
		public ActionResult<String> Post(MemeInfo memeInfo)
		{
			var meme = new Meme(memeInfo);
			MemeRepo.Add(meme);
			return Ok(meme.Guid.ToString());
		}

		[Route("{id}")]
		[HttpGet]
		public ActionResult GetByID(Guid id)
		{
			if (HttpContext.WebSockets.IsWebSocketRequest)
			{
				var resource = MemeRepo.Get(id);
				if (resource == null)
				{
					return new NotFoundResult();
				}
				WebSocket socket = HttpContext.WebSockets.AcceptWebSocketAsync().Result;

				if (socket != null && socket.State == WebSocketState.Open)
				{
					while (!HttpContext.RequestAborted.IsCancellationRequested)
					{
						bool completed = false;
						int num = -1;
						while (!completed)
						{
							if (resource.WorkStatus.Percentage == num)
								continue;
							if (resource.WorkStatus.Status == WorkStatus.Done)
								completed = true;
							num = resource.WorkStatus.Percentage;
							String response;
							if(!completed)
								response = $"{num:D2}\n";
							else
								response = "DONE\n";
							var bytes = System.Text.Encoding.UTF8.GetBytes(response);
							socket.SendAsync(new System.ArraySegment<byte>(bytes),
								WebSocketMessageType.Text, true, CancellationToken.None);
						}
					}
				}
			}
			return new StatusCodeResult(101);
		}
	}
}