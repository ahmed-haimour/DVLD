using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointmentDetails;

public sealed class GetTestAppointmentDetailsQueryHandler(
    IAppDbContext context,
    IIdentityService identityService)
    : IRequestHandler<GetTestAppointmentDetailsQuery, Result<TestAppointmentDetailsResponse>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<TestAppointmentDetailsResponse>> Handle(
        GetTestAppointmentDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var row = await _context.TestAppointments
            .AsNoTracking()
            .Where(appointment => appointment.Id == request.TestAppointmentId)
            .Select(appointment => new
            {
                appointment.Id,
                appointment.LocalDrivingLicenseApplicationId,
                appointment.TestTypeId,
                TestTypeName = appointment.TestType.TestTypeTitle,
                appointment.AppointmentDate,
                TestAppointmentFees = appointment.PaidFees,
                RetakeApplicationFees = appointment.RetakeTestApplication != null
                    ? (decimal?)appointment.RetakeTestApplication.PaidFees
                    : null,
                appointment.IsLocked,
                appointment.RetakeTestApplicationId,
                appointment.CreatedByUserId,
                TestResult = appointment.Test != null ? appointment.Test.TestResult : (bool?)null,
                TestNotes = appointment.Test != null ? appointment.Test.Notes : null,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return Result<TestAppointmentDetailsResponse>.Failure(ApplicationErrors.TestAppointmentNotFound);
        }

        var userResult = await _identityService.GetUserByIdAsync(row.CreatedByUserId, cancellationToken);
        var createdByUserName = userResult.IsSuccess
            ? userResult.Value.UserName ?? string.Empty
            : string.Empty;

        var response = new TestAppointmentDetailsResponse(
            row.Id,
            row.LocalDrivingLicenseApplicationId,
            row.TestTypeId,
            row.TestTypeName,
            row.AppointmentDate,
            row.TestAppointmentFees,
            row.RetakeApplicationFees,
            row.TestAppointmentFees + (row.RetakeApplicationFees ?? 0m),
            row.IsLocked,
            row.RetakeTestApplicationId,
            createdByUserName,
            row.TestResult,
            row.TestNotes);

        return Result<TestAppointmentDetailsResponse>.Success(response);
    }
}
