using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components.Small
{
    public class WorksCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public WorksCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            if (!User.IsInRole("Admin"))
            {
                var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
                var count = await _manager.WorkService.GetAllWorksCountOfOneUser(userId!);
                if (count > 99)
                {
                    return "99+";
                }
                return count.ToString();
            }
            else
            {
                var count = await _manager.WorkService.GetWorksCountForSidebarAsync();
                if (int.TryParse(count, out int parsedCount) && parsedCount > 99)
                {
                    return "99+";
                }
                return count;
            }

        }
    }
}
