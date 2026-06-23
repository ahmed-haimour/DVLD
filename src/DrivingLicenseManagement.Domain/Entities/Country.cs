using DrivingLicenseManagement.Domain.Common;

namespace DrivingLicenseManagement.Domain.Entities;

public class Country : Entity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Person> People { get; set; } = new List<Person>();
}
