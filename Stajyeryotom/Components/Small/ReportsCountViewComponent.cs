using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

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
            var count = await _manager.ReportService.GetAllReportsCountAsync();
            return count.ToString();
        }
    }
}
