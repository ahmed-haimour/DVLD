using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class LocalDrivingLicenseApplicationConfiguration : IEntityTypeConfiguration<LocalDrivingLicenseApplication>
{
    public void Configure(EntityTypeBuilder<LocalDrivingLicenseApplication> builder)
    {
        builder.ToTable("LocalDrivingLicenseApplications");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Application)
            .WithOne(x => x.LocalDrivingLicenseApplication)
            .HasForeignKey<LocalDrivingLicenseApplication>(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LicenseClass)
            .WithMany(x => x.LocalDrivingLicenseApplications)
            .HasForeignKey(x => x.LicenseClassId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
