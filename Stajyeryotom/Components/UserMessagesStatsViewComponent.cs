using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components
{
    public class UserMessagesStatsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public UserMessagesStatsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);

            var stat = await _manager.MessageService.GetUserMessagesStatsAsync(userId!);
            return View(stat);
        }
    }
}
