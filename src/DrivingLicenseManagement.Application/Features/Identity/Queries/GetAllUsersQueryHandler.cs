using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Queries;

public sealed class GetAllUsersQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetAllUsersQuery, Result<PaginatedList<UserResponse>>>
{
    private readonly IIdentityService _identityService = identityService;

    // Filtering, paging and the join to People all happen in IdentityService, where the
    // AppUser IQueryable and the Person navigation are reachable.
    public Task<Result<PaginatedList<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        return _identityService.GetAllUsersAsync(
            request.FilterBy, request.FilterValue, request.PageNumber, request.PageSize, ct);
    }
}
