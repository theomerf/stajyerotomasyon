using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components.Small
{
    public class ApplicationsCountViewComponent
    {
        private readonly IServiceManager _manager;

        public ApplicationsCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            var count = await _manager.ApplicationService.GetAllApplicationsCountAsync();
            if(count > 99)
            {
                return "99+";
            }
            return count.ToString();
        }
    }
}
