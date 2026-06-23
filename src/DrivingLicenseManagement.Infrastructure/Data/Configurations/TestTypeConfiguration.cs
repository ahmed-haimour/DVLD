using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class TestTypeConfiguration : IEntityTypeConfiguration<TestType>
{
    public void Configure(EntityTypeBuilder<TestType> builder)
    {
        builder.ToTable("TestTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TestTypeTitle).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TestTypeDescription).HasMaxLength(500).IsRequired();
        builder.Property(x => x.TestTypeFees).HasPrecision(18, 2);
    }
}
