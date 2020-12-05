using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repositories;

namespace WebApplication.Controllers
{

	[Route("meme")]
	[ApiController]
	public class MemeGenerator : ControllerBase
	{
		private readonly MemoryMemeRepository _memeRepo;

		public MemeGenerator(IMemeRepository memeRepository)
		{
			_memeRepo = memeRepository as MemoryMemeRepository;
		}

		[HttpPost]
		[Route("create")]
		public ActionResult<String> Create(MemeInfo memeInfo)
		{
			var meme = new Meme(memeInfo);
			_memeRepo.Add(meme);
			return Ok(meme.Guid.ToString());
		}

		[Route("repo.{format}"), FormatFilter]
		[HttpGet]
		public List<Meme> GetRepo()
		{
			return _memeRepo.Memes;
		}

		[HttpGet]
		[Route("delete/{guid}")]
		public ActionResult DeleteMeme(Guid guid)
		{
			Meme meme = _memeRepo.Get(guid);
			if(meme == null)
				return new NotFoundResult();
			_memeRepo.Delete(guid);
			return Ok();
		}
		
		[Route("view/{guid}")]
		[HttpGet]
		public async Task<ActionResult> GetMemeByGuid(Guid guid)
		{
			Meme meme = _memeRepo.Get(guid);
			if(meme == null)
				return new NotFoundResult();
			return PhysicalFile(meme.FilePath, "application/octet-stream", enableRangeProcessing: true);
		}
	}
}