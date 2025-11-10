using Microsoft.AspNetCore.Mvc;

namespace LoginApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            bool isLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";

            if (!isLoggedIn)
            {
                return RedirectToAction("Login", "Account");
            }

            if (TempData.ContainsKey("LoginSuccessMessage"))
            {
                ViewBag.Message = TempData["LoginSuccessMessage"]?.ToString();
                TempData.Remove("LoginSuccessMessage");
            }

            ViewBag.IsLoggedIn = isLoggedIn;
            return View();
        }
    }
}
