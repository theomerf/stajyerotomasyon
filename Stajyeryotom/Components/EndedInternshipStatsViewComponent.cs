using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Contracts;

namespace Stajyeryotom.Components
{
    public class EndedInternshipStatsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public EndedInternshipStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var endedInternshipStats = await _manager.AuthService.GetEndedInternshipStatsAsync();

            return View(endedInternshipStats);
        }
    }
}
