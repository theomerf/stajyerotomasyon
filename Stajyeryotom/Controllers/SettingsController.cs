using Microsoft.AspNetCore.Mvc;

namespace Stajyeryotom.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return PartialView("_Index");
        }
    }
}
