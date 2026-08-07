using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class InternationalLicenseConfiguration : IEntityTypeConfiguration<InternationalLicense>
{
    public void Configure(EntityTypeBuilder<InternationalLicense> builder)
    {
        builder.ToTable("InternationalLicenses");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Application)
            .WithOne(x => x.InternationalLicense)
            .HasForeignKey<InternationalLicense>(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Driver)
            .WithMany(x => x.InternationalLicenses)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.IssuedUsingLocalLicense)
            .WithMany(x => x.InternationalLicenses)
            .HasForeignKey(x => x.IssuedUsingLocalLicenseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
