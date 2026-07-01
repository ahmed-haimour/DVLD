using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DomainApplication = DrivingLicenseManagement.Domain.Entities.Application;

namespace DrivingLicenseManagement.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    public DbSet<DomainApplication> Applications => Set<DomainApplication>();
    public DbSet<ApplicationType> ApplicationTypes => Set<ApplicationType>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<DetainedLicense> DetainedLicenses => Set<DetainedLicense>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<LicenseClass> LicenseClasses => Set<LicenseClass>();
    public DbSet<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications => Set<LocalDrivingLicenseApplication>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestAppointment> TestAppointments => Set<TestAppointment>();
    public DbSet<TestType> TestTypes => Set<TestType>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
