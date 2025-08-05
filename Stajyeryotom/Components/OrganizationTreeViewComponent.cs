using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components
{
    public class OrganizationTreeViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public OrganizationTreeViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var organization = await _manager.DepartmentService.GetAllDepartmentsAsync();
            return View(organization);
        }
    }
}
