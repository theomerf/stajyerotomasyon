using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components.Small
{
    public class SectionsCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public SectionsCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            var count = await _manager.SectionService.GetAllSectionsCountAsync();

            return count;
        }
    }
}
