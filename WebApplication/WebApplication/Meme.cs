using System;
using System.IO;
using WebApplication.Models;

namespace WebApplication
{
	[Serializable]
	public class Meme
	{
		protected internal String FilePath
		{
			get
			{
				return Path.Combine(Startup.ContentRoot, "Videos", $"{Guid}.mp4");
			}
		}

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
			WorkStatus = new MemeWork(this);
		}
		
		public void ReadXml(System.Xml.XmlReader reader)
		{
			WorkStatus.Meme = this;
		}
	}
}