using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.LicenseClasses.Dtos;
using DrivingLicenseManagement.Application.Features.LicenseClasses.Mappers;
using DrivingLicenseManagement.Application.Features.LicenseClasses.Queries.GetLicenseClasses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrivingLicenseManagement.Application.Features.LicenseClasses.Queries;

public class GetLicenseClassesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLicenseClassesQuery, Result<IReadOnlyList<LicenseClassDto>>>
{
        private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<LicenseClassDto>>> Handle(GetLicenseClassesQuery request, CancellationToken ct)
    {
        var licenseClasses = await _context.LicenseClasses
        .AsNoTracking()
        .OrderBy(licenseClass => licenseClass.ClassName)
        .Select(licenseClass => new LicenseClassDto(
            licenseClass.Id,
            licenseClass.ClassName,
            licenseClass.ClassDescription,
            licenseClass.MinimumAllowedAge,
            licenseClass.DefaultValidityLength,
            licenseClass.ClassFees))
            .ToListAsync(ct);

        return Result<IReadOnlyList<LicenseClassDto>>
            .Success(licenseClasses);
    }
}