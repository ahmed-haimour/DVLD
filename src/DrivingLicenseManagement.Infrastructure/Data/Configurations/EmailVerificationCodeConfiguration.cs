using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class EmailVerificationCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
        builder.ToTable("EmailVerificationCodes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CodeHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.SentAt)
            .IsRequired();

        builder.Property(x => x.FailedAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        // The same UserId cannot appear twice in EmailVerificationCodes.
        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<EmailVerificationCode>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}