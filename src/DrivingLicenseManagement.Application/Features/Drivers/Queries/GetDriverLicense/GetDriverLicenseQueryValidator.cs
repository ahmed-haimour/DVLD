using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDriverLicense;

public sealed class GetDriverLicenseQueryValidator : AbstractValidator<GetDriverLicenseQuery>
{
    public GetDriverLicenseQueryValidator()
    {
        RuleFor(query => query.LicenseId)
            .NotEmpty().WithMessage("License id is required.");
    }
}
