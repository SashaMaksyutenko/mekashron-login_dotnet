using Microsoft.AspNetCore.Mvc;

namespace LoginApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (TempData.ContainsKey("LoginSuccessMessage"))
            {
                ViewBag.Message = TempData["LoginSuccessMessage"]?.ToString();
                TempData.Remove("LoginSuccessMessage");
            }
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            return View();
        }
    }
}
