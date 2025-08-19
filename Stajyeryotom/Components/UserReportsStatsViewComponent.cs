using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components
{
    public class UserReportsStatsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public UserReportsStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);

            var stat = await _manager.ReportService.GetUserReportsStatsAsync(userId!);
            return View(stat);
        }
    }
}
