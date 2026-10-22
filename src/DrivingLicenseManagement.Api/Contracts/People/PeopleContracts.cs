using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Enums;
using DrivingLicenseManagement.Domain.Enums;
using DrivingLicenseManagement.Application.Common.Models;
namespace DrivingLicenseManagement.Api.Contracts.People;


// Requests
public sealed record PersonRequest(
    string NationalNumber,
    string FirstName,
    string SecondName,
    string? ThirdName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string Address,
    string Phone,
    string? Email,
    Guid NationalityCountryId,
    string? ImagePath);


public sealed record GetPeopleRequest(
    PersonFilterBy FilterBy = PersonFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10);

public sealed record CreatePersonResponse(Guid PersonId);


// Responses & Mappers

public sealed record PersonResponse(
    Guid Id,
    string NationalNumber,
    string FirstName,
    string SecondName,
    string? ThirdName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string Address,
    string Phone,
    string? Email,
    Guid NationalityCountryId,
    string? ImagePath);


public sealed record GetPeopleResponse(
    IReadOnlyCollection<PersonResponse> Items,
    int PageNumber,
    int TotalPages,
    int TotalCount);


public static class PeopleContractsMapper
{
    public static PersonResponse ToResponse(this PersonDto person)
        => new(
            Id: person.Id,
            NationalNumber: person.NationalNumber,
            FirstName: person.FirstName,
            SecondName: person.SecondName,
            ThirdName: person.ThirdName,
            LastName: person.LastName,
            DateOfBirth: person.DateOfBirth,
            Gender: person.Gender,
            Address: person.Address,
            Phone: person.Phone,
            Email: person.Email,
            NationalityCountryId: person.NationalityCountryId,
            ImagePath: person.ImagePath);

    public static GetPeopleResponse ToResponse(this PaginatedList<PersonDto> people)
        => new(Items: people.Items!
                .Select(x => x.ToResponse())
                .ToList(),
            PageNumber: people.PageNumber,
            TotalPages: people.TotalPages,
            TotalCount: people.TotalCount);
}