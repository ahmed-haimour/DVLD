using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Drivers.Queries.GetLicensesByPersonId;

public sealed class GetLicensesByPersonIdQueryValidator : AbstractValidator<GetLicensesByPersonIdQuery>
{
    public GetLicensesByPersonIdQueryValidator()
    {
        RuleFor(query => query.PersonId)
            .NotEmpty().WithMessage("Person id is required.");
    }
}
