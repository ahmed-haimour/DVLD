using DrivingLicenseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainApplication = DrivingLicenseManagement.Domain.Entities.Application;

namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<DomainApplication> Applications { get; }
    public DbSet<ApplicationType> ApplicationTypes { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<DetainedLicense> DetainedLicenses { get; }
    public DbSet<Driver> Drivers { get; }
    public DbSet<License> Licenses { get; }
    public DbSet<LicenseClass> LicenseClasses { get; }
    public DbSet<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications { get; }
    public DbSet<Person> People { get; }
    public DbSet<Test> Tests { get; }
    public DbSet<TestAppointment> TestAppointments { get; }
    public DbSet<TestType> TestTypes { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
