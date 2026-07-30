using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using DrivingLicenseManagement.Application.Features.Identity.Enums;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Queries;

public sealed record GetAllUsersQuery(
    UserFilterBy FilterBy = UserFilterBy.None,
    string? FilterValue = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<UserResponse>>>;
