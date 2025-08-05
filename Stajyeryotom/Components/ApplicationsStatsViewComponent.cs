using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components
{
    public class ApplicationsStatsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public ApplicationsStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stats = await _manager.ApplicationService.GetApplicationsStatusStatsAsync();
            return View(stats);
        }
    }
}
