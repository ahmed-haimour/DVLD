using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DrivingLicenseManagement.Infrastructure.Data;

public static class ApplicationTypeSeeder
{
    // Application types the business logic depends on, keyed by their fixed ids. Titles are
    // seeded defaults that administrators may edit; only the ids are used by business logic.
    // Fees are placeholders (0) and are meant to be configured through UpdateApplicationType.
    private static readonly (Guid Id, string Title)[] SeedData =
    [
        (ApplicationTypeIds.NewLocalDrivingLicense, "New Local Driving License Service"),
        (ApplicationTypeIds.RetakeTest, "Retake Test"),
        (ApplicationTypeIds.RenewLicense, "Renew License"),
        (ApplicationTypeIds.ReplacementForDamagedLicense, "Replacement for a Damaged Driving License"),
        (ApplicationTypeIds.ReplacementForLostLicense, "Replacement for a Lost Driving License"),
        (ApplicationTypeIds.NewInternationalLicense, "New International Driving License"),
    ];

    public static async Task SeedApplicationTypesAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var (id, title) in SeedData)
        {
            bool exists = await context.ApplicationTypes.AnyAsync(applicationType => applicationType.Id == id);

            if (!exists)
            {
                context.ApplicationTypes.Add(new ApplicationType(id)
                {
                    ApplicationTypeTitle = title,
                    ApplicationTypeFees = 0m,
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
