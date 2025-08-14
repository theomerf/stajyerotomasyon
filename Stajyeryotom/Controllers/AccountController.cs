using Entities.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Stajyeryotom.Models;
using System.Security.Claims;

namespace Stajyeryotom.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IServiceManager _manager;

        public AccountController(IServiceManager manager, UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _manager = manager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var login = new LoginModel();
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (ModelState.IsValid && model.Name != null)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null && user.UserName != null && model.Password != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: false
                    );
                    if (result.Succeeded)
                    {
                        user.LastLoginDate = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);

                        var claims = new List<Claim>
                        {
                            new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        };

                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                        await HttpContext.SignInAsync(
                            IdentityConstants.ApplicationScheme,
                            new ClaimsPrincipal(claimsIdentity)
                        );

                        if (isAjaxRequest)
                        {
                            return Json(new
                            {
                                success = true,
                                message = "Giriş başarılı!",
                                redirectUrl = Url.Action("Index", "Home")
                            });
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (isAjaxRequest)
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Kullanıcı no veya şifre hatalı."
                            });
                        }

                        ModelState.AddModelError("Login.Name", "Kullanıcı no veya şifre hatalı.");
                    }
                }
                else
                {
                    if (isAjaxRequest)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Kullanıcı no veya şifre hatalı."
                        });
                    }

                    ModelState.AddModelError("Login.Name", "Kullanıcı no veya şifre hatalı.");
                }
            }
            else
            {
                if (isAjaxRequest)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new {
                            Key = x.Key,
                            Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                        })
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Form doğrulama hatası.",
                        error = errors
                    });
                }
            }

            if (isAjaxRequest)
            {
                return Json(new
                {
                    success = false,
                    message = "Bilinmeyen bir hata oluştu."
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                return Json(new
                {
                    success = true,
                    message = "Başarıyla çıkış yapıldı."
                });
            }

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
