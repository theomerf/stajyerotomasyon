using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components
{
    public class UserDetailsViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public UserDetailsViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userName = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.Name);
            var user = await _manager.AuthService.GetOneUserAsync(userName);

            return View(user);
        }
    }
}
