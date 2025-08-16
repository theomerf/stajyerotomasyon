using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components.Small
{
    public class ReportsCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public ReportsCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            if (!User.IsInRole("Admin"))
            {
                var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
                var count = await _manager.ReportService.GetAllReportsCountOfOneUserForSidebarAsync(userId!);

                if (int.Parse(count) > 99)
                {
                    return "99+";
                }
                return count;
            }
            else
            {
                var count = await _manager.ReportService.GetReportsCountForSidebarAsync();

                if (int.Parse(count) > 99)
                {
                    return "99+";
                }
                return count;
            }
        }
    }
}
