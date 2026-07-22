using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Identity.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Identity.Queries;

public sealed class GetAllUsersQueryHandler(
    IIdentityService identityService,
    IAppDbContext context)
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserResponse>>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var users = await _identityService.GetAllUsersAsync(ct);

        var personIds = users.Select(u => u.PersonId).ToList();
        var people = await _context.People
            .AsNoTracking()
            .Where(p => personIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        var responses = users.Select(u =>
        {
            people.TryGetValue(u.PersonId, out var person);
            var fullName = person is null ? string.Empty : string.Join(" ", new[]
            {
                person.FirstName,
                person.SecondName,
                person.ThirdName,
                person.LastName
            }.Where(name => !string .IsNullOrWhiteSpace(name)));

            return new UserResponse(
                UserId: u.Id,
                PersonId: u.PersonId,
                UserName: u.UserName ?? string.Empty,
                FullName: fullName,
                Email: u.Email,
                Role: u.Roles.FirstOrDefault() ?? string.Empty,
                IsActive: u.IsActive);
        }).ToList();

        return Result<IReadOnlyList<UserResponse>>.Success(responses);
    }
}
