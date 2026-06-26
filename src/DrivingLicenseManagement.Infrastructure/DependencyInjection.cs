using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Infrastructure.Data;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrivingLicenseManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = "cache";
            options.TableName = "AppCache";
        });

        services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10), // L2 (SQL)
            LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1 (in-memory)
        });

        return services;
    }
}
