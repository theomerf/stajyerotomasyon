using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Contracts;

namespace Stajyeryotom.Components.Small
{
    public class InternsCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public InternsCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            var internCount = await _manager.AuthService.GetInternsCountAsync();
            return internCount;
        }
    }
}
