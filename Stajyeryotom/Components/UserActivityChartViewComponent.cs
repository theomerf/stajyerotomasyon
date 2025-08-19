using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;
using System.Text.Json;

namespace Stajyeryotom.Components
{
    public class UserActivityChartViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public UserActivityChartViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
            var reportsStats = await _manager.ReportService.GetDailyReportsCountOfOneUser(userId!);

            ViewBag.DailyDataJson = JsonSerializer.Serialize(reportsStats);
            return View();
        }
    }
}
