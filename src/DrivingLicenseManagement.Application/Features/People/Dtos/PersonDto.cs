
using DrivingLicenseManagement.Domain.Enums;

namespace DrivingLicenseManagement.Application.Features.People.Dtos;

public sealed record PersonDto(
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
