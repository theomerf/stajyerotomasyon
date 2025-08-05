using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Contracts;

namespace Stajyeryotom.Components
{
    public class InternsSummaryViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public InternsSummaryViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var internStats = await _manager.AuthService.GetInternshipStatsAsync();

            return View(internStats);
        }
    }
}
