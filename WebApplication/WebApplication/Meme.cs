using System;
using WebApplication.Models;

namespace WebApplication
{
	public class Meme
	{
		protected internal String FilePath => Utils.FilePath(this);

		public String CatText;
		public String DrummerText;
		public String DrumText;
		public Guid Guid;
		public MemeWork MemeWork;
		public DateTime CreationDate;

		public Meme(){}
		public Meme(MemeInfo memeInfo)
		{
			CatText = memeInfo.CatText;
			DrummerText = memeInfo.DrummerText;
			DrumText = memeInfo.DrumText;
			Guid = new Guid(Utils.MD5($"{CatText}{DrummerText}{DrumText}"));
			MemeWork = new MemeWork(this);
			CreationDate = DateTime.Now;
		}
	}
}