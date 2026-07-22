using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Queries;

public sealed record GetAllUsersQuery
: IRequest<Result<IReadOnlyList<UserResponse>>>;