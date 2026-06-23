using DrivingLicenseManagement.Domain.Common;
using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Domain.Entities;

public class Person : Entity
{
    public string NationalNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string? ThirdName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; } // ---- check later if email is required or not -----
    public Guid NationalityCountryId { get; set; }
    public string? ImagePath { get; set; }
    public Country NationalityCountry { get; set; } = null!;
    public Driver? Driver { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
