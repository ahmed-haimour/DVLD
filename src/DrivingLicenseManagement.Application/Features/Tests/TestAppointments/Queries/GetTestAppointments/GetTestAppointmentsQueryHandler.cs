using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointments;

public sealed class GetTestAppointmentsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetTestAppointmentsQuery, Result<IReadOnlyList<TestAppointmentDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<TestAppointmentDto>>> Handle(
        GetTestAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _context.TestAppointments
            .AsNoTracking()
            .Where(appointment => appointment.LocalDrivingLicenseApplicationId == request.LocalDrivingLicenseApplicationId
                && appointment.TestTypeId == request.TestTypeId)
            .OrderBy(appointment => appointment.AppointmentDate)
            .Select(appointment => new TestAppointmentDto(
                appointment.Id,
                appointment.TestTypeId,
                appointment.AppointmentDate,
                appointment.PaidFees,
                appointment.IsLocked,
                appointment.Test != null ? appointment.Test.TestResult : (bool?)null))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<TestAppointmentDto>>.Success(appointments);
    }
}
