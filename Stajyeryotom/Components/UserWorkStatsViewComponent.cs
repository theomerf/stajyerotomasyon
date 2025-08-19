using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components
{
    public class UserWorkStatsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public UserWorkStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userWorkStats = await _manager.WorkService.GetUserWorksStatsAsync(userId!);

            return View(userWorkStats);
        }
    }
}
