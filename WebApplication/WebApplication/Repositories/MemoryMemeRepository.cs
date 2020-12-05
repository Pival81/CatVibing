using System;
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

		private int RunningCount => _Memes.Count(x => x.WorkStatus.Status == WorkStatus.Working);

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
			if (RunningCount < 5)
				meme.WorkStatus.StartWork();
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
			var target = _Memes.FirstOrDefault(r => r.Guid == memeId);
			_Memes.Remove(target);
			File.Delete(target.FilePath);
			Save();
		}
	}
}