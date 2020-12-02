using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace WebApplication
{
	public class MemoryMemeRepository
	{
		private IList<Meme> _Memes { get; set; }

		public List<Meme> Memes
		{
			get
			{
				return _Memes as List<Meme>;
			}
		}

		private int RunningCount
		{
			get
			{
				return _Memes.Count(x => x.WorkStatus.Status == WorkStatus.Working);
			}
		}

		public MemoryMemeRepository()
		{
			try
			{
				IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
					.UseOptimizedNamespaces()
					.EnableImplicitTyping(this.GetType())
					.Create();
				var xml = File.ReadAllText("Database.xml");
				MemoryMemeRepository xmlrepo = serializer.Deserialize<MemoryMemeRepository>(
					new XmlReaderSettings{IgnoreWhitespace = false}, xml);
				_Memes = xmlrepo._Memes;
			}
			catch (Exception ex)
			{
				_Memes = new List<Meme>();
			}
		}
 
		public IEnumerable<Meme> Get() => _Memes;
 
		public Meme Get(Guid memeId)
		{
			return _Memes.FirstOrDefault(x => x.Guid == memeId);
		}
		
		public void Add(Meme meme)
		{
			_Memes.Add(meme);
			if (RunningCount < 5)
			{
				meme.WorkStatus.StartWork();
			}
			Save();
		}

		public void Save()
		{
			var xml = Utils.GetXmlFromObject(this);
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