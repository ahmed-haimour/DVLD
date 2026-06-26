using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPersonByNationalNo;

public sealed class GetPersonByNationalNoQueryValidator : AbstractValidator<GetPersonByNationalNoQuery>
{
    public GetPersonByNationalNoQueryValidator()
    {
        RuleFor(query => query.NationalNumber)
            .NotEmpty().WithMessage("National number is required.")
            .MaximumLength(20).WithMessage("National number must not exceed 20 characters.");
    }
}
