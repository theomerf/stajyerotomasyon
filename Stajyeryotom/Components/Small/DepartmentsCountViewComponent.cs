using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components.Small
{
    public class DepartmentsCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public DepartmentsCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            var count = await _manager.DepartmentService.GetAllDepartmentsCountAsync();

            return count;
        }
    }
}
