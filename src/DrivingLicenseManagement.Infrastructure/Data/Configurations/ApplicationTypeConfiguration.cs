using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
{
    public void Configure(EntityTypeBuilder<ApplicationType> builder)
    {
        builder.ToTable("ApplicationTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApplicationTypeTitle).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ApplicationTypeFees).HasPrecision(18, 2);
    }
}
