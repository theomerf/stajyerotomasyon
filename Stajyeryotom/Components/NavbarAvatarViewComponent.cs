using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components
{
    public class NavbarAvatarViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public NavbarAvatarViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.Name);
            var user = await _manager.AuthService.GetOneUserAsync(userId!);
            return user.ProfilePictureUrl ?? "profile_pictures/default.png";
        }
    }
}
