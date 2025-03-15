using Microsoft.AspNetCore.Mvc;

namespace StockAPI.Controllers
{
	public class StockController : ControllerBase
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
