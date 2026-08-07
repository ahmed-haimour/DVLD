using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DrivingLicenseManagement.Infrastructure.Data;

public static class TestTypeSeeder
{
    // The three test types the workflow depends on, keyed by their fixed ids. Fees are
    // placeholders (0) and are meant to be configured later through UpdateTestType.
    private static readonly (Guid Id, string Title, string Description)[] SeedData =
    [
        (TestTypeIds.VisionTest, "Vision Test", "Checks the applicant's eyesight."),
        (TestTypeIds.WrittenTest, "Written Test", "Theoretical written driving test."),
        (TestTypeIds.PracticalStreetTest, "Practical Street Test", "On-road practical driving test."),
    ];

    public static async Task SeedTestTypesAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var (id, title, description) in SeedData)
        {
            bool exists = await context.TestTypes.AnyAsync(testType => testType.Id == id);

            if (!exists)
            {
                context.TestTypes.Add(new TestType(id)
                {
                    TestTypeTitle = title,
                    TestTypeDescription = description,
                    TestTypeFees = 0m,
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
