using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        // builder.HasIndex(x => x.PersonId).IsUnique();

        builder.HasOne(x => x.Person)
            .WithOne()
            .HasForeignKey<AppUser>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
