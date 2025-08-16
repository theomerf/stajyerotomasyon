using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Components.Small
{
    public class MessagesCountViewComponent : ViewComponent
    {
        private readonly IServiceManager _manager;

        public MessagesCountViewComponent(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<String> InvokeAsync()
        {
            if (!User.IsInRole("Admin"))
            {
                var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
                var count = await _manager.MessageService.GetAllMessagesCountForOneUserAsync(userId!);
                
                if (int.Parse(count) > 99)
                {
                    return "99+";
                }
                return count;
            }
            else
            {
                var count = await _manager.MessageService.GetAllMessagesCountAsync();
                if (int.Parse(count) > 99)
                {
                    return "99+";
                }
                return count;
            }

        }
    }
}
