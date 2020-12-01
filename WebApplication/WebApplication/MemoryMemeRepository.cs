using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApplication
{
	public class MemoryMemeRepository
	{
		private IList<Meme> _Memes { get; set; }

		private int RunningCount
		{
			get
			{
				return _Memes.Count(x => x.WorkStatus.Status == WorkStatus.Working);
			}
		}

		public MemoryMemeRepository()
		{
			_Memes = new List<Meme>();
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
		}
 
         
		public void Delete(Guid memeId)
		{
			var target = _Memes.FirstOrDefault(r => r.Guid == memeId);
			_Memes.Remove(target);
			File.Delete(target.FilePath);
		}
	}
}