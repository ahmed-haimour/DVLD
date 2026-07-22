using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IAppDbContext context,
    IIdentityService identityService,
    ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<CreateUserCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
        .FirstOrDefaultAsync(
            p => p.Id == request.PersonId,
            cancellationToken);

        if (person is null)
        {
            _logger.LogWarning("Person with id {PersonId} was not found.", request.PersonId);

            return Result<Guid>.Failure(ApplicationErrors.PersonNotFound);
        }

        bool personHasAccount = await _identityService.PersonHasUserAccountAsync(request.PersonId, cancellationToken);

        if (personHasAccount)
        {
            _logger.LogWarning("Person with id {PersonId} already has a user account.", request.PersonId);

            return Result<Guid>.Failure(ApplicationErrors.PersonAlreadyHasUserAccount);
        }


        if (!string.IsNullOrWhiteSpace(person.Email) &&
            !string.Equals(
            person.Email,
            request.Email,
            StringComparison.OrdinalIgnoreCase))
        {
            return Result<Guid>.Failure(ApplicationErrors.PersonEmailMismatch);
        }

        var result = await _identityService.CreateUserAsync(
            request.PersonId,
            request.UserName.Trim(),
            request.Email.Trim(),
            request.Password,
            request.Role,
            cancellationToken);

        if (result.IsError)
        {
            return result.Error!.Value;
        }

        _logger.LogInformation("User with id {UserId} was created successfully.", result.Value.Id);

        return Result<Guid>.Success(result.Value.Id);
    }
}
