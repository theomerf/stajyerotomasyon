using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Stajyeryotom.Infrastructure.Extensions
{
    public static class ApplicationExtension
    {
        public static void ConfigureAndCheckMigration(this IApplicationBuilder app)
        {
            RepositoryContext context = app
            .ApplicationServices
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<RepositoryContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        public static void ConfigureLocalization(this WebApplication app)
        {
            app.UseRequestLocalization(options =>
            {
                options.AddSupportedCultures("tr-TR")
                .AddSupportedUICultures("tr-TR")
                .SetDefaultCulture("tr-TR");
            }
            );
        }

        public static async Task ConfigureDefaultAdminUser(this IApplicationBuilder app)
        {
            const string adminUser = "0";
            const string adminPassword = "Admin+123456";

            UserManager<Account> userManager = app
                .ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<UserManager<Account>>();

            RoleManager<IdentityRole> roleManager = app
                .ApplicationServices
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            Account? user = await userManager.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new Account()
                {
                    FirstName = "Admin",
                    LastName = "Root",
                    BirthDate = DateTime.UtcNow,
                    Email = "omerfarukyalcin08@gmail.com",
                    PhoneNumber = "05425946284",
                    UserName = adminUser
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Admin kullanıcısı oluşturulmadı");
                }

                var roleResult = await userManager.AddToRolesAsync(user,
                    await roleManager
                    .Roles
                    .Where(r => r.Name != "Stajyer")
                    .Select(r => r.NormalizedName!)
                    .ToListAsync()
                );

                if (!roleResult.Succeeded)
                {
                    throw new Exception("Sistem admin kullanıcısının rollerini verirken hata oluştu.");
                }
            }

        }

        public static async Task ConfigureCsvAsync(this IApplicationBuilder app)
        {
            await app.ImportDataFromCsvAsync(
                "wwwroot/database/departments.csv",
                "wwwroot/database/sections.csv",
                "wwwroot/database/accounts.csv",
                "wwwroot/database/applications.csv"
            );
        }

    }
}
