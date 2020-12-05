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

		[Route("repo.{format}"), FormatFilter]
		[HttpGet]
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
				return new NotFoundResult();
			_memeRepo.Delete(guid);
			return Ok();
		}

		[Route("getstatus/{guid}")]
		[HttpGet]
		public ActionResult<String> GetStatus(Guid guid)
		{
			var meme = _memeRepo.Get(guid);
			return Ok(meme.MemeWork.Status);
		}
		
		[Route("get/{guid}")]
		[HttpGet]
		public ActionResult<Meme> GetMeme(Guid guid)
		{
			var meme = _memeRepo.Get(guid);
			return Ok(meme);
		}
		
		[Route("view/{guid}")]
		[HttpGet]
		public ActionResult GetMemeByGuid(Guid guid)
		{
			Meme meme = _memeRepo.Get(guid);
			if(meme == null)
				return new NotFoundResult();
			return PhysicalFile(meme.FilePath, "application/octet-stream", enableRangeProcessing: true);
		}
	}
}