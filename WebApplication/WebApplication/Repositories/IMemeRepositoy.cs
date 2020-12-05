using System;
using System.Collections.Generic;

namespace WebApplication.Repositories
{
	public interface IMemeRepository
	{
		List<Meme> Memes { get; }
		Meme Get(Guid memeId);
		void Add(Meme meme);
		void Save();
		void Delete(Guid memeId);
	}
}