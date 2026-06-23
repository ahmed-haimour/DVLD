using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("Licenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.PaidFees).HasPrecision(18, 2);
        builder.Property(x => x.IssueReason).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(x => x.Application)
            .WithOne(x => x.License)
            .HasForeignKey<License>(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Driver)
            .WithMany(x => x.Licenses)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LicenseClass)
            .WithMany(x => x.Licenses)
            .HasForeignKey(x => x.LicenseClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
