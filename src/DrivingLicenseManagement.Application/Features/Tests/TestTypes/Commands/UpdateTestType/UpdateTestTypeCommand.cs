using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.TestTypes.Commands.UpdateTestType;

public sealed record UpdateTestTypeCommand(
    Guid Id,
    string TestTypeTitle,
    string TestTypeDescription,
    decimal TestTypeFees) : IRequest<Result>;
