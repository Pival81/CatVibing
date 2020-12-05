﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtendedXmlSerializer;

namespace WebApplication.Repositories
{
	public class MemoryMemeRepository : IMemeRepository
	{
		private IList<Meme> _Memes { get; set; }

		public List<Meme> Memes => _Memes as List<Meme>;

		private int RunningCount => _Memes.Count(x => x.MemeWork.Status == WorkStatus.Working);

		public MemoryMemeRepository()
		{
			try 
			{
				var xml = File.ReadAllText("Database.xml");
				var xmlrepo = Utils.DefaultXmlSerializer()
					.Deserialize<List<Meme>>(xml);
				_Memes = xmlrepo;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_Memes = new List<Meme>();
			}
		}

		public Meme Get(Guid memeId)
		{
			return _Memes.FirstOrDefault(x => x.Guid == memeId);
		}
		
		public void Add(Meme meme)
		{
			_Memes.Add(meme);
			Next();
			Save();
		}

		public void Next()
		{
			if (RunningCount >= 5)
				return;
			var queue = new Queue<Meme>(
				from meme in _Memes
				where meme.MemeWork.Status == WorkStatus.Scheduled
				orderby meme.CreationDate
				select meme);
			for (int i = 0; i < 5 - RunningCount; i++)
			{
				Meme meme;
				if (queue.TryDequeue(out meme))
					meme.MemeWork.StartWork();
				else
					break;
			}
			Save();
		}
		
		public void Save()
		{
			var xml = Utils.GetXmlFromObject(Memes);
			using (StreamWriter outputFile = new StreamWriter(Path.Combine(Startup.ContentRoot, "Database.xml")))
			{
				outputFile.WriteLine(xml);
			}
		}
		
		public void Delete(Guid memeId)
		{
			var meme = _Memes.FirstOrDefault(r => r.Guid == memeId);
			_Memes.Remove(meme);
			meme.MemeWork.StopWork();
			File.Delete(meme.FilePath);
			Next();
			Save();
		}
	}
}