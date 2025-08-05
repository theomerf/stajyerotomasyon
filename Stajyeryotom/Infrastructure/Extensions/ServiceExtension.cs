using Entities.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Contracts;
using Services;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Services;
using System.Reflection;

namespace Stajyeryotom.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("mssqlconnection"),
                b => b.MigrationsAssembly("Stajyeryotom"));

                options.EnableSensitiveDataLogging(true);
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<Account, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureSession(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "stajyeryotom.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(10);

            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void ConfigureRepositoryRegistration(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
        }

        public static void ConfigureServiceRegistration(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IAuthService, AuthManager>();
            services.AddScoped<IDepartmentService, DepartmentManager>();
            services.AddScoped<ISectionService, SectionManager>();
            services.AddScoped<IApplicationService,ApplicationManager>();
            services.AddScoped<IEventService, EventManager>();
        }

        public static void ConfigureApplicationCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.SlidingExpiration = true;

                options.Events.OnSigningIn = context =>
                {

                    if (context.Properties.IsPersistent)
                    {
                        context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30);
                    }

                    return Task.CompletedTask;
                };
            });

        }

        public static void ConfigureRouting(this IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                options.AppendTrailingSlash = false;
            });
        }

        public static void ConfigureCsv(this IServiceCollection services)
        {
            services.AddScoped<CsvImporter>();
        }
    }
}
