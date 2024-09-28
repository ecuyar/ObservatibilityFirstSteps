using Microsoft.AspNetCore.Mvc;

namespace OrderAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		[HttpGet("getNumber")]
		public int GetNumber()
		{
			return new Random().Next(minValue: 0, maxValue: 101);
		}
	}
}
