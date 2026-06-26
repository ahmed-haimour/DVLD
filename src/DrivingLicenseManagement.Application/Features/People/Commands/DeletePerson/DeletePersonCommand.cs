using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.People.Commands.DeletePerson;

public sealed record DeletePersonCommand(Guid Id) : IRequest<Result>;
