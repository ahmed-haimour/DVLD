using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.People.Commands.UpdatePerson;

public sealed record UpdatePersonCommand(
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
    string? ImagePath) : IRequest<Result>;
