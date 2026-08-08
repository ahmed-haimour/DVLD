using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetInternationalLicenseDriver;

public sealed class GetInternationalLicenseDriverQueryValidator : AbstractValidator<GetInternationalLicenseDriverQuery>
{
    public GetInternationalLicenseDriverQueryValidator()
    {
        RuleFor(query => query.InternationalLicenseId)
            .NotEmpty().WithMessage("International license id is required.");
    }
}
