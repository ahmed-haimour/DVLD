using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IAppDbContext context)
    : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IIdentityService _identityService = identityService;
    private readonly IAppDbContext _context = context;

    public async Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId == Guid.Empty)
            return ApplicationErrors.UserNotAuthenticated;

        var userResult = await _identityService.GetUserByIdAsync(_currentUser.UserId, ct);
        if (userResult.IsError)
            return userResult.Error!;

        var user = userResult.Value;

        var person = await _context.People
            .AsNoTracking()
            .Include(p => p.NationalityCountry)
            .FirstOrDefaultAsync(p => p.Id == user.PersonId, ct);

        if (person is null)
            return ApplicationErrors.PersonNotFound;

        var fullName = string.Join(" ", new[]
        {
            person.FirstName,
            person.SecondName,
            person.ThirdName,
            person.LastName
        }.Where(name => !string.IsNullOrWhiteSpace(name)));

        return new CurrentUserResponse(
            UserId: user.Id,
            UserName: user.UserName ?? string.Empty,
            IsActive: user.IsActive,
            Roles: [.. user.Roles],
            PersonId: user.PersonId,
            NationalNo: person.NationalNumber,
            FullName: fullName,
            DateOfBirth: person.DateOfBirth,
            Gender: person.Gender,
            Address: person.Address,
            Phone: person.Phone,
            Email: person.Email,
            Country: person.NationalityCountry.Name,
            ImagePath: person.ImagePath);
    }
}
