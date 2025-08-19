using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stajyeryotom.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sidebar()
        {
            return PartialView("_Sidebar");
        }

        public IActionResult Navbar()
        {
            return PartialView("_Navbar");
        }
    }
}
