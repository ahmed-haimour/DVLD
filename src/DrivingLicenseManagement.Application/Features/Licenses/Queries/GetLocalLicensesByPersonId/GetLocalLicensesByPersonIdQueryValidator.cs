using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Licenses.Queries.GetLocalLicensesByPersonId;

public sealed class GetLocalLicensesByPersonIdQueryValidator : AbstractValidator<GetLocalLicensesByPersonIdQuery>
{
    public GetLocalLicensesByPersonIdQueryValidator()
    {
        RuleFor(query => query.PersonId)
            .NotEmpty().WithMessage("Person id is required.");
    }
}
