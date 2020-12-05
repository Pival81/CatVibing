using System;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using WebApplication.Repositories;

namespace WebApplication.Controllers
{
	public class FleckTask : Task
	{

		public FleckTask(IWebSocketConnection socket, CancellationToken ct, IMemeRepository memeRepository) 
			: base(() => DoWork(socket, memeRepository), ct){ }

		private static void DoWork(IWebSocketConnection socket, IMemeRepository memeRepo)
		{
			var id = socket.ConnectionInfo.Path.Substring(1);
			Meme resource = null;
			try { resource = memeRepo.Get(new Guid(id)); }
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
		}
	}
}