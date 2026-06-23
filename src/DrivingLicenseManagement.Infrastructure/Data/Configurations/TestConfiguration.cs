using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Tests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasOne(x => x.TestAppointment)
            .WithOne(x => x.Test)
            .HasForeignKey<Test>(x => x.TestAppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
