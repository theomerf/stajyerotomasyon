using Microsoft.AspNetCore.Mvc;

namespace Stajyeryotom.Components
{
    public class ReportsStatsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
