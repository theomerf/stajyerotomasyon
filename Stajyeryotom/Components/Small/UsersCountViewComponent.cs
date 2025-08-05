using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Stajyeryotom.Components.Small
{
    public class UsersCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public UsersCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<string> InvokeAsync()
        {
            var count = await _manager.AuthService.GetAllUsersCountAsync();

            return count;
        }
    }
}
