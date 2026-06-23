using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class LicenseClassConfiguration : IEntityTypeConfiguration<LicenseClass>
{
    public void Configure(EntityTypeBuilder<LicenseClass> builder)
    {
        builder.ToTable("LicenseClasses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClassName).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.ClassName).IsUnique();
        builder.Property(x => x.ClassDescription).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ClassFees).HasPrecision(18, 2);
    }
}
