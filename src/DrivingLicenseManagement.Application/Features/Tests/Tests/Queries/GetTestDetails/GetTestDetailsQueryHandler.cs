using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Tests.Tests.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Queries.GetTestDetails;

public sealed class GetTestDetailsQueryHandler(
    IAppDbContext context,
    IIdentityService identityService)
    : IRequestHandler<GetTestDetailsQuery, Result<TestDetailsDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<TestDetailsDto>> Handle(
        GetTestDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var row = await _context.Tests
            .AsNoTracking()
            .Where(test => test.Id == request.TestId)
            .Select(test => new
            {
                test.Id,
                test.TestAppointmentId,
                test.TestAppointment.TestTypeId,
                TestTypeName = test.TestAppointment.TestType.TestTypeTitle,
                test.TestResult,
                test.Notes,
                test.TestAppointment.AppointmentDate,
                test.CreatedByUserId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return Result<TestDetailsDto>.Failure(ApplicationErrors.TestNotFound);
        }

        var userResult = await _identityService.GetUserByIdAsync(row.CreatedByUserId, cancellationToken);
        var createdByUserName = userResult.IsSuccess
            ? userResult.Value.UserName ?? string.Empty
            : string.Empty;

        var response = new TestDetailsDto(
            row.Id,
            row.TestAppointmentId,
            row.TestTypeId,
            row.TestTypeName,
            row.TestResult,
            row.Notes,
            row.AppointmentDate,
            createdByUserName);

        return Result<TestDetailsDto>.Success(response);
    }
}
