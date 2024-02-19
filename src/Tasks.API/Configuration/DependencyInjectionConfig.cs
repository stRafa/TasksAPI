using Tasks.Application.Interfaces;
using Tasks.Application.Services;
using Tasks.Domain.Mission;
using Tasks.Domain.Passport;
using Tasks.Domain.User;
using Tasks.Infrastructure.Repositories;

namespace Tasks.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<IUserRepository>(provider => new UserRepository(connectionString));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMissionRepository>(p => new MissionRepository(connectionString));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPassportRepository>(p => new PassportRepository(connectionString));
    }
}