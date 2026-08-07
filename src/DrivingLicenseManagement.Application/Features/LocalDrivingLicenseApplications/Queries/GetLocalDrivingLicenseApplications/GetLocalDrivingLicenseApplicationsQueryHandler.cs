using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Dtos;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Enums;
using DrivingLicenseManagement.Domain.Entities;
using DrivingLicenseManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplications;

public sealed class GetLocalDrivingLicenseApplicationsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLocalDrivingLicenseApplicationsQuery, Result<PaginatedList<LocalDrivingLicenseApplicationListItemResponse>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<LocalDrivingLicenseApplicationListItemResponse>>> Handle(
        GetLocalDrivingLicenseApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<LocalDrivingLicenseApplication> query = _context.LocalDrivingLicenseApplications
            .AsNoTracking();

        var filterValue = request.FilterValue?.Trim();

        if (request.FilterBy != LocalDrivingLicenseApplicationFilterBy.None && !string.IsNullOrWhiteSpace(filterValue))
        {
            switch (request.FilterBy)
            {
                case LocalDrivingLicenseApplicationFilterBy.LocalDrivingLicenseApplicationId:
                    // Search screens prefer empty results over a validation error, so an
                    // id that isn't a valid GUID simply matches nothing.
                    query = Guid.TryParse(filterValue, out var id)
                        ? query.Where(ldl => ldl.Id == id)
                        : query.Where(ldl => false);
                    break;

                case LocalDrivingLicenseApplicationFilterBy.NationalNo:
                    query = query.Where(ldl => ldl.Application.ApplicantPerson.NationalNumber.Contains(filterValue));
                    break;

                case LocalDrivingLicenseApplicationFilterBy.FullName:
                    // Every typed word must appear in some name column, so "ahmad ali" matches
                    // Ahmad Ali Hassan. Each token becomes its own ANDed Where.
                    foreach (var token in filterValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Where(ldl =>
                            ldl.Application.ApplicantPerson.FirstName.Contains(token) ||
                            ldl.Application.ApplicantPerson.SecondName.Contains(token) ||
                            (ldl.Application.ApplicantPerson.ThirdName != null && ldl.Application.ApplicantPerson.ThirdName.Contains(token)) ||
                            ldl.Application.ApplicantPerson.LastName.Contains(token));
                    }

                    break;

                case LocalDrivingLicenseApplicationFilterBy.Status:
                    // TryParse also accepts numeric strings, so IsDefined rejects out-of-range values.
                    // An unrecognized status matches nothing rather than failing validation.
                    query = Enum.TryParse<ApplicationStatus>(filterValue, ignoreCase: true, out var status) && Enum.IsDefined(status)
                        ? query.Where(ldl => ldl.Application.ApplicationStatus == status)
                        : query.Where(ldl => false);
                    break;
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Projecting the four name columns lets EF emit the joins without materializing whole rows.
        var rows = await query
            .OrderByDescending(ldl => ldl.Application.ApplicationDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ldl => new
            {
                ldl.Id,
                LicenseClassName = ldl.LicenseClass.ClassName,
                ldl.Application.ApplicantPerson.NationalNumber,
                ldl.Application.ApplicantPerson.FirstName,
                ldl.Application.ApplicantPerson.SecondName,
                ldl.Application.ApplicantPerson.ThirdName,
                ldl.Application.ApplicantPerson.LastName,
                ldl.Application.ApplicationDate,
                Status = ldl.Application.ApplicationStatus,
                // Count passed test results (a test type can only be passed once), not attempts.
                PassedTests = ldl.TestAppointments.Count(ta => ta.Test != null && ta.Test.TestResult),
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(row => new LocalDrivingLicenseApplicationListItemResponse(
            row.Id,
            row.LicenseClassName,
            row.NationalNumber,
            string.Join(" ", new[] { row.FirstName, row.SecondName, row.ThirdName, row.LastName }
                .Where(name => !string.IsNullOrWhiteSpace(name))),
            row.ApplicationDate,
            row.PassedTests,
            row.Status)).ToList();

        var result = new PaginatedList<LocalDrivingLicenseApplicationListItemResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Items = items
        };

        return Result<PaginatedList<LocalDrivingLicenseApplicationListItemResponse>>.Success(result);
    }
}
