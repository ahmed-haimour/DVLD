using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery
: IRequest<Result<CurrentUserResponse>>;
