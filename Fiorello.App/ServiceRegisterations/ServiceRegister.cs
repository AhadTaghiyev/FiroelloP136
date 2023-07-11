using System;
using Fiorello.App.Context;
using Fiorello.App.Services.Implementations;
using Fiorello.App.Services.Interfaces;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.App.ServiceRegisterations
{
	public static class ServiceRegister
	{
		public static void Register(this IServiceCollection service,IConfiguration config)
		{
			service.AddScoped<IBasketService, BasketService>();
			service.AddScoped<IMailService, MailService>();
			service.AddIdentity<AppUser, IdentityRole>()
                   .AddDefaultTokenProviders()
                   .AddEntityFrameworkStores<FiorelloDbContext>();
			service.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });
            service.AddDbContext<FiorelloDbContext>(opt =>
             {
                 opt.UseSqlServer(config.GetConnectionString("Default"));
             });
        }
	}
}

