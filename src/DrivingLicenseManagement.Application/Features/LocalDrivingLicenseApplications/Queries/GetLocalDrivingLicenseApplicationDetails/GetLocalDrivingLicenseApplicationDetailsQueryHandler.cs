using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplicationDetails;

public sealed class GetLocalDrivingLicenseApplicationDetailsQueryHandler(
    IAppDbContext context,
    IIdentityService identityService,
    ILogger<GetLocalDrivingLicenseApplicationDetailsQueryHandler> logger)
    : IRequestHandler<GetLocalDrivingLicenseApplicationDetailsQuery, Result<LocalDrivingLicenseApplicationDetailsResponse>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<GetLocalDrivingLicenseApplicationDetailsQueryHandler> _logger = logger;

    public async Task<Result<LocalDrivingLicenseApplicationDetailsResponse>> Handle(
        GetLocalDrivingLicenseApplicationDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var row = await _context.LocalDrivingLicenseApplications
            .AsNoTracking()
            .Where(ldl => ldl.Id == request.LocalDrivingLicenseApplicationId)
            .Select(ldl => new
            {
                ldl.Id,
                ldl.ApplicationId,
                LicenseClassName = ldl.LicenseClass.ClassName,
                ldl.Application.ApplicationStatus,
                ldl.Application.ApplicationDate,
                ldl.Application.LastStatusDate,
                ldl.Application.PaidFees,
                ApplicationType = ldl.Application.ApplicationType.ApplicationTypeTitle,
                ldl.Application.ApplicantPersonId,
                ldl.Application.ApplicantPerson.FirstName,
                ldl.Application.ApplicantPerson.SecondName,
                ldl.Application.ApplicantPerson.ThirdName,
                ldl.Application.ApplicantPerson.LastName,
                ldl.Application.CreatedByUserId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            _logger.LogWarning(
                "Local driving license application with id {LocalDrivingLicenseApplicationId} was not found.",
                request.LocalDrivingLicenseApplicationId);

            return Result<LocalDrivingLicenseApplicationDetailsResponse>
                .Failure(ApplicationErrors.LocalDrivingLicenseApplicationNotFound);
        }

        var userResult = await _identityService.GetUserByIdAsync(row.CreatedByUserId, cancellationToken);
        var createdByUserName = userResult.IsSuccess
            ? userResult.Value.UserName ?? string.Empty
            : string.Empty;

        var applicantFullName = string.Join(" ", new[] { row.FirstName, row.SecondName, row.ThirdName, row.LastName }
            .Where(name => !string.IsNullOrWhiteSpace(name)));

        var response = new LocalDrivingLicenseApplicationDetailsResponse(
            row.Id,
            row.ApplicationId,
            row.LicenseClassName,
            // Passed-test count is deferred until the Test feature exists.
            PassedTests: 0,
            row.ApplicationStatus,
            row.ApplicationDate,
            row.LastStatusDate,
            row.PaidFees,
            row.ApplicationType,
            row.ApplicantPersonId,
            applicantFullName,
            createdByUserName);

        return Result<LocalDrivingLicenseApplicationDetailsResponse>.Success(response);
    }
}
