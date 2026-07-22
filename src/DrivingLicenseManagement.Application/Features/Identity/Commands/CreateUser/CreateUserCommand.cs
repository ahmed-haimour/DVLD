using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.CreateUser;

public sealed record CreateUserCommand(
    Guid PersonId,
    string UserName,
    string Email,
    string Password,
    string Role = Roles.User) : IRequest<Result<Guid>>;
