using Microsoft.AspNetCore.Mvc;

namespace Stajyeryotom.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return PartialView("_Index");
        }
    }
}
