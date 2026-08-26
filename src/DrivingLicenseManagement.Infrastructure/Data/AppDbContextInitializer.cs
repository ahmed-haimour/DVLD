using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Infrastructure.Data;

public static class AppDbContextInitializerExtensions
{
    // Single startup entry point: applies the migrations that already exist in the assembly and
    // then runs every seeder, so a fresh database comes up complete without any manual step.
    public static async Task InitialiseDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
    }
}

public class AppDbContextInitializer(
    ILogger<AppDbContextInitializer> logger,
    AppDbContext context,
    IServiceProvider services)
{
    private readonly ILogger<AppDbContextInitializer> _logger = logger;
    private readonly AppDbContext _context = context;
    private readonly IServiceProvider _services = services;

    // Applies pending migrations only - the schema is defined by the migration files that are
    // already checked in; nothing here creates or generates migrations.
    public async Task InitialiseAsync()
    {
        try
        {
            var pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();

            if (pendingMigrations.Count == 0)
            {
                _logger.LogInformation("Database schema is up to date; no pending migrations.");

                return;
            }

            _logger.LogInformation(
                "Applying {Count} pending migration(s): {Migrations}",
                pendingMigrations.Count,
                string.Join(", ", pendingMigrations));

            await _context.Database.MigrateAsync();

            _logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");

            throw;
        }
    }

    // Delegates to the existing seeders, each of which owns its own seed data and skips rows that
    // are already present, so restarting the API never produces duplicates.
    public async Task SeedAsync()
    {
        try
        {
            await _services.SeedRolesAsync();
            await _services.SeedApplicationTypesAsync();
            await _services.SeedLicenseClassesAsync();
            await _services.SeedTestTypesAsync();

            _logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");

            throw;
        }
    }
}
