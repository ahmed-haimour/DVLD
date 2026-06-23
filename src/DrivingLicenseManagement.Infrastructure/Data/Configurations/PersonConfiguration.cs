using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrivingLicenseManagement.Infrastructure.Data.Configurations;

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NationalNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.NationalNumber).IsUnique();
        builder.Property(x => x.FirstName).HasMaxLength(20).IsRequired();
        builder.Property(x => x.SecondName).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ThirdName).HasMaxLength(20);
        builder.Property(x => x.LastName).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Gender).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(50);
        builder.Property(x => x.ImagePath).HasMaxLength(250);

        builder.HasOne(x => x.NationalityCountry)
            .WithMany(x => x.People)
            .HasForeignKey(x => x.NationalityCountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
