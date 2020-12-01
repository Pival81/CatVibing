using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using Xabe.FFmpeg;
using IStream = System.Runtime.InteropServices.ComTypes.IStream;

namespace WebApplication
{
	public class Meme
	{
		public String FilePath;
		public String CatText;
		public String DrummerText;
		public String DrumText;
		public Guid Guid;
		public MemeWork WorkStatus;

		public Meme(){}
		public Meme(MemeInfo memeInfo)
		{
			CatText = memeInfo.CatText;
			DrummerText = memeInfo.DrummerText;
			DrumText = memeInfo.DrumText;
			Guid = Guid.NewGuid();
			FilePath = Path.Combine(Startup.ContentRoot, "Videos", $"{Guid}.mp4");
			WorkStatus = new MemeWork(this);
		}
	}
}