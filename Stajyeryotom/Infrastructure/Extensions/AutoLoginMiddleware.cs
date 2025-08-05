using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Stajyeryotom.Infrastructure.Extensions
{
    public class AutoLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() && !context!.User!.Identity!.IsAuthenticated)
            {
                var user = await userManager.FindByEmailAsync("omerfarukyalcin08@gmail.com");
                if (user != null)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                }
            }

            await _next(context);
        }
    }
}

