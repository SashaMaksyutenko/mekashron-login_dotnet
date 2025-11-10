using Microsoft.AspNetCore.Mvc;

namespace LoginApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = TempData["LoginSuccessMessage"]?.ToString();
            return View();
        }
    }
}