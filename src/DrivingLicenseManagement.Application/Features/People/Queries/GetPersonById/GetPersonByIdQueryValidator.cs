using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPersonById;

public sealed class GetPersonByIdQueryValidator : AbstractValidator<GetPersonByIdQuery>
{
    public GetPersonByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage("Person Id is required.");
    }
}