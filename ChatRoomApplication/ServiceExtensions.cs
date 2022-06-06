using ChatRoomApplication.Databases;
using ChatRoomApplication.Services;
using ChatRoomApplication.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatRoomApplication
{
    public static class ServiceExtensions
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IBotService, BotService>();
        }

        public static void AddDatabaseContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            services.AddDefaultIdentity<IdentityUser>()
               .AddEntityFrameworkStores<ApplicationDbContext>();
        }

    }
}
