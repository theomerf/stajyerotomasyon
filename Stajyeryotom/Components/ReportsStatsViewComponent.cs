using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components
{
    public class ReportsStatsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public ReportsStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stats = await _manager.ReportService.GetReportsStatusStatsAsync();
            return View(stats);
        }
    }
}
