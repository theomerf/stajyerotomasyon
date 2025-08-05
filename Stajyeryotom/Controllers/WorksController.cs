using Microsoft.AspNetCore.Mvc;

namespace Stajyeryotom.Controllers
{
    public class WorksController : Controller
    {
        public IActionResult Index()
        {
            return PartialView("_Index");
        }
    }
}
