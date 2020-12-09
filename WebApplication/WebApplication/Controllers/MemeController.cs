using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repositories;

namespace WebApplication.Controllers
{
	[Route("meme")]
	[ApiController]
	public class MemeController : ControllerBase
	{
		private readonly IMemeRepository _memeRepo;

		public MemeController(IMemeRepository memeRepository)
		{
			_memeRepo = memeRepository;
		}

		[Route("create")]
		[HttpPost]
		public ActionResult<String> Create(MemeInfo memeInfo)
		{
			var guid = Utils.MD5($"{memeInfo.CatText}{memeInfo.DrummerText}{memeInfo.DrumText}");
			var tempMeme = _memeRepo.Get(new Guid(guid));
			if (tempMeme != null)
				return Ok($"{tempMeme.Guid}:{tempMeme.MemeWork.Status}");
			var meme = new Meme(memeInfo);
			_memeRepo.Add(meme);
			return Ok($"{meme.Guid}:{meme.MemeWork.Status}");
		}

		[Route("repo")]
		[HttpGet]
		[Produces("application/json")]
		public List<Meme> GetRepo()
		{
			_memeRepo.Save();
			return _memeRepo.Memes;
		}

		[Route("delete/{guid}")]
		[HttpGet]
		public ActionResult DeleteMeme(Guid guid)
		{
			Meme meme = _memeRepo.Get(guid);
			if(meme == null)
				return NotFound(null);
			_memeRepo.Delete(guid);
			return Ok();
		}

		[Route("getstatus/{guid}")]
		[HttpGet]
		public ActionResult<String> GetStatus(Guid guid)
		{
			var meme = _memeRepo.Get(guid);
			if (meme == null)
				return NotFound(null);
			return Ok(meme.MemeWork.Status);
		}
		
		[Route("get/{guid}")]
		[HttpGet]
		public ActionResult<Meme> GetMemeInfo(Guid guid)
		{
			var meme = _memeRepo.Get(guid);
			if (meme == null)
				return NotFound(null);
			return Ok(meme);
		}
		
		[Route("watch/{guid}")]
		[HttpGet]
		public ActionResult GetMemeByGuid(Guid guid)
		{
			Meme meme = _memeRepo.Get(guid);
			if(meme == null)
				return NotFound(null);
			return PhysicalFile(meme.FilePath, "application/octet-stream", enableRangeProcessing: true);
		}
	}
}