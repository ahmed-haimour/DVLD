using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DrivingLicenseManagement.Infrastructure.Data;

public static class LicenseClassSeeder
{
    // The license classes, keyed by their fixed ids. Business logic references these ids
    // (e.g. Class 3 for international-license eligibility); titles/fees remain editable data.
    private static readonly (Guid Id, string Name, string Description, byte MinimumAllowedAge, byte DefaultValidityLength, decimal Fees)[] SeedData =
    [
        (LicenseClassIds.SmallMotorcycle, "Small Motorcycle",
            "It allows the driver to drive small motorcycles, It is suitable for motorcycles with small capacity and limited power.", 18, 5, 15.00m),
        (LicenseClassIds.HeavyMotorcycle, "Heavy Motorcycle License",
            "Heavy Motorcycle License (Large Motorcycle License)", 21, 5, 30.00m),
        (LicenseClassIds.OrdinaryDrivingLicense, "Ordinary Driving License",
            "Ordinary driving license (car licence)", 18, 10, 20.00m),
        (LicenseClassIds.Commercial, "Commercial",
            "Commercial driving license (taxi/limousine)", 21, 10, 200.00m),
        (LicenseClassIds.Agricultural, "Agricultural",
            "Agricultural and work vehicles used in farming or construction (tractors / tillage machinery)", 21, 10, 50.00m),
        (LicenseClassIds.SmallAndMediumBus, "Small and Medium Bus",
            "Small and medium bus license", 21, 10, 250.00m),
        (LicenseClassIds.TruckAndHeavyVehicle, "Truck and Heavy Vehicle",
            "Truck and heavy vehicle license", 21, 10, 300.00m),
    ];

    public static async Task SeedLicenseClassesAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var (id, name, description, minimumAllowedAge, defaultValidityLength, fees) in SeedData)
        {
            bool exists = await context.LicenseClasses.AnyAsync(licenseClass => licenseClass.Id == id);

            if (!exists)
            {
                context.LicenseClasses.Add(new LicenseClass(id)
                {
                    ClassName = name,
                    ClassDescription = description,
                    MinimumAllowedAge = minimumAllowedAge,
                    DefaultValidityLength = defaultValidityLength,
                    ClassFees = fees,
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
