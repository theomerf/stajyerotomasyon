using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stajyeryotom.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        public IActionResult Index()
        {
            return PartialView("_Index");
        }
    }
}
