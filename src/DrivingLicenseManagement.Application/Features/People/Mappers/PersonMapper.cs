using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Domain.Entities;

namespace DrivingLicenseManagement.Application.Features.People.Mappers;

public static class PersonMapper
{
    public static PersonDto ToDto(this Person person)
    {
        return new PersonDto(
            person.Id,
            person.NationalNumber,
            person.FirstName,
            person.SecondName,
            person.ThirdName,
            person.LastName,
            person.DateOfBirth,
            person.Gender,
            person.Address,
            person.Phone,
            person.Email,
            person.NationalityCountryId,
            person.ImagePath);
    }

    public static List<PersonDto> ToDtos(this IEnumerable<Person> people)
    {
        return [.. people.Select(person => person.ToDto())];
    }
}