using DrivingLicenseManagement.Application.Features.People.Enums;
using DrivingLicenseManagement.Domain.Enums;
using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.People.Queries.GetPeople;

public sealed class GetPeopleQueryValidator : AbstractValidator<GetPeopleQuery>
{
    public GetPeopleQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(query => query.FilterBy)
            .IsInEnum().WithMessage("Invalid filter field.");

        When(query => query.FilterBy != PersonFilterBy.None, () =>
        {
            RuleFor(query => query.FilterValue)
                .NotEmpty().WithMessage("A filter value is required when a filter field is selected.")
                .MaximumLength(100).WithMessage("Filter value must not exceed 100 characters.");

            // RuleFor(query => query.FilterValue)
            //     .Must(value => Guid.TryParse(value, out _))
            //     .When(query => query.FilterBy == PersonFilterBy.PersonId)
            //     .WithMessage("Person id must be a valid GUID.");

            // RuleFor(query => query.FilterValue)
            //     .Must(value => Enum.TryParse<Gender>(value, ignoreCase: true, out var gender) && Enum.IsDefined(gender))
            //     .When(query => query.FilterBy == PersonFilterBy.Gender)
            //     .WithMessage("Gender must be either 'Male' or 'Female'.");
        });
    }
}
