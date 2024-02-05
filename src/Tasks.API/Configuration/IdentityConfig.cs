using Microsoft.AspNetCore.Identity;
using Tasks.Infrastructure.Identity;

namespace Tasks.API.Configuration;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();

        services.AddJwtConfiguration(configuration);

        return services;
    }
}