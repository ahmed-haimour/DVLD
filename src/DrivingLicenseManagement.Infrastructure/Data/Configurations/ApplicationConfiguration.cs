using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainApplication = DrivingLicenseManagement.Domain.Entities.Application;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<DomainApplication>
{
    public void Configure(EntityTypeBuilder<DomainApplication> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApplicationStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.PaidFees).HasPrecision(18, 2);

        builder.HasOne(x => x.ApplicantPerson)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.ApplicantPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApplicationType)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.ApplicationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
