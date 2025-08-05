using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stajyeryotom.Components
{
    public class DashboardChartsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public DashboardChartsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var departmentCounts = await _manager.AuthService.GetInternsDepartmentAsync();
            var applicationsStats = await _manager.ApplicationService.GetMonthlyApplicationCountsAsync();

            var chartData = new
            {
                labels = departmentCounts.Keys.ToArray(),
                datasets = new[]
                {
                        new
                        {
                            data = departmentCounts.Values.ToArray(),
                            backgroundColor = new[]
                            {
                                "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0",
                                "#9966FF", "#FF6B6B", "#4ECDC4", "#45B7D1",
                                "#96CEB4", "#FFEAA7", "#DDA0DD", "#98D8C8"
                            },
                            borderColor = "#fff",
                            borderWidth = 2
                        }
                    }
            };

            ViewBag.ChartData = JsonSerializer.Serialize(chartData);
            ViewBag.MonthlyDataJson = JsonSerializer.Serialize(applicationsStats);
            return View();

        }
    }
}
