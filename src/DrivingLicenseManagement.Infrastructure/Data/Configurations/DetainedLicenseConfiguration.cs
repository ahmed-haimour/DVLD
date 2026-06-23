using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class DetainedLicenseConfiguration : IEntityTypeConfiguration<DetainedLicense>
{
    public void Configure(EntityTypeBuilder<DetainedLicense> builder)
    {
        builder.ToTable("DetainedLicenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FineFees).HasPrecision(18, 2);


        builder.HasOne(x => x.License)
            .WithMany(x => x.Detentions)
            .HasForeignKey(x => x.LicenseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReleaseApplication)
            .WithOne(x => x.ReleasedDetainedLicense)
            .HasForeignKey<DetainedLicense>(x => x.ReleaseApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.ReleasedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
