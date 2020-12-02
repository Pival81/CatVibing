using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{

	[Route("meme")]
	[ApiController]
	public class MemeGenerator : ControllerBase
	{
		public static MemoryMemeRepository MemeRepo = new MemoryMemeRepository();

		[HttpPost]
		[Route("create")]
		public ActionResult<String> Post(MemeInfo memeInfo)
		{
			var meme = new Meme(memeInfo);
			MemeRepo.Add(meme);
			return Ok(meme.Guid.ToString());
		}

		[Route("repo.{format}"), FormatFilter]
		[HttpGet]
		public MemoryMemeRepository GetRepo()
		{
			return MemeRepo;
		}

		[HttpGet]
		[Route("delete/{guid}")]
		public ActionResult DeleteMeme(Guid guid)
		{
			Meme meme = MemeRepo.Get(guid);
			if(meme == null)
				return new NotFoundResult();
			MemeRepo.Delete(guid);
			return Ok();
		}
		
		[Route("view/{guid}")]
		[HttpGet]
		public async Task<ActionResult> GetMemeByGuid(Guid guid)
		{
			Meme meme = MemeRepo.Get(guid);
			if(meme == null)
				return new NotFoundResult();
			return PhysicalFile(meme.FilePath, "application/octet-stream", enableRangeProcessing: true);
		}
	}
}