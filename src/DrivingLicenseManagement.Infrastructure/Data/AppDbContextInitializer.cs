using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
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
    // Default person and the admin account that belongs to them. The person is matched by
    // national number (uniquely indexed) so restarting the API never inserts a duplicate.
    private const string DefaultNationalNumber = "N10000";
    private const string DefaultNationalityCountryName = "Jordan";
    private const string DefaultUserName = "ahmed.haimour";
    private const string DefaultUserEmail = "ahmedhaimour@gmail.com";
    private const string DefaultUserPassword = "Admin@123";

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
            await _services.SeedCountriesAsync();
            await _services.SeedApplicationTypesAsync();
            await _services.SeedLicenseClassesAsync();
            await _services.SeedTestTypesAsync();

            await SeedDefaultPersonAsync();

            _logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");

            throw;
        }
    }

    // Seeds the single default person together with their admin account. Runs after the roles and
    // countries seeders because it depends on both.
    private async Task SeedDefaultPersonAsync()
    {
        using var scope = _services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var person = await _context.People
            .FirstOrDefaultAsync(person => person.NationalNumber == DefaultNationalNumber);

        if (person is null)
        {
            var nationality = await _context.Countries
                .FirstOrDefaultAsync(country => country.Name == DefaultNationalityCountryName)
                ?? throw new InvalidOperationException(
                    $"Country '{DefaultNationalityCountryName}' was not found; countries must be seeded first.");

            person = new Person
            {
                NationalNumber = DefaultNationalNumber,
                FirstName = "Ahmed",
                SecondName = "Mahmoud",
                ThirdName = "Issa",
                LastName = "Haimour",
                DateOfBirth = new DateTime(1995, 5, 17),
                Gender = Gender.Male,
                Address = "Amman, Jordan",
                Phone = "0791234567",
                Email = DefaultUserEmail,
                NationalityCountryId = nationality.Id,
            };

            _context.People.Add(person);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded the default person {NationalNumber}.", DefaultNationalNumber);
        }

        if (await userManager.Users.AnyAsync(user => user.PersonId == person.Id))
            return;

        // The account may already exist against a different person - for example after the seed
        // constants were edited. Identity would reject the duplicate and the exception would fail
        // startup on every run, so leave the existing account alone instead.
        var conflicting = await userManager.FindByEmailAsync(DefaultUserEmail)
            ?? await userManager.FindByNameAsync(DefaultUserName);

        if (conflicting is not null)
        {
            _logger.LogWarning(
                "Skipped seeding the default account: '{UserName}' already uses that email or user name.",
                conflicting.UserName);

            return;
        }

        var user = new AppUser
        {
            PersonId = person.Id,
            UserName = DefaultUserName,
            Email = DefaultUserEmail,
            IsActive = true,
            EmailConfirmed = true,
        };

        var createResult = await userManager.CreateAsync(user, DefaultUserPassword);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed the default user: {string.Join("; ", createResult.Errors.Select(error => error.Description))}");
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.Admin);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign the '{Roles.Admin}' role to the default user: {string.Join("; ", roleResult.Errors.Select(error => error.Description))}");
        }

        _logger.LogInformation("Seeded the default {Role} account {UserName}.", Roles.Admin, DefaultUserName);
    }
}
