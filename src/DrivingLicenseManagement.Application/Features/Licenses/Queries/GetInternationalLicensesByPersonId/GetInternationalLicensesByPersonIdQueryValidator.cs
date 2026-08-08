using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Licenses.Queries.GetInternationalLicensesByPersonId;

public sealed class GetInternationalLicensesByPersonIdQueryValidator : AbstractValidator<GetInternationalLicensesByPersonIdQuery>
{
    public GetInternationalLicensesByPersonIdQueryValidator()
    {
        RuleFor(query => query.PersonId)
            .NotEmpty().WithMessage("Person id is required.");
    }
}
