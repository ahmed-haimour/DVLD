using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class TestAppointmentConfiguration : IEntityTypeConfiguration<TestAppointment>
{
    public void Configure(EntityTypeBuilder<TestAppointment> builder)
    {
        builder.ToTable("TestAppointments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PaidFees).HasPrecision(18, 2);

        builder.HasOne(x => x.TestType)
            .WithMany(x => x.TestAppointments)
            .HasForeignKey(x => x.TestTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LocalDrivingLicenseApplication)
            .WithMany(x => x.TestAppointments)
            .HasForeignKey(x => x.LocalDrivingLicenseApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RetakeTestApplication)
            .WithOne(x => x.RetakeTestAppointment)
            .HasForeignKey<TestAppointment>(x => x.RetakeTestApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
