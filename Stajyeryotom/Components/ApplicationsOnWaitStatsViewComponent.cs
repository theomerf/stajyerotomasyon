using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components
{
    public class ApplicationsOnWaitStatsViewComponent :ViewComponent
    {
        private readonly IServiceManager _manager;

        public ApplicationsOnWaitStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stats = await _manager.ApplicationService.GetApplicationsOnWaitStatsAsync();
            return View(stats);
        }
    }
}
