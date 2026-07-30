using DrivingLicenseManagement.Application.Features.Identity.Enums;
using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Identity.Queries;

public sealed class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(query => query.FilterBy)
            .IsInEnum().WithMessage("Invalid filter field.");

        When(query => query.FilterBy != UserFilterBy.None, () =>
        {
            RuleFor(query => query.FilterValue)
                .NotEmpty().WithMessage("A filter value is required when a filter field is selected.")
                .MaximumLength(100).WithMessage("Filter value must not exceed 100 characters.");

            RuleFor(query => query.FilterValue)
                .Must(value => Guid.TryParse(value, out _))
                .When(query => query.FilterBy is UserFilterBy.UserId or UserFilterBy.PersonId)
                .WithMessage("The id must be a valid GUID.");

            RuleFor(query => query.FilterValue)
                .Must(value => bool.TryParse(value, out _))
                .When(query => query.FilterBy == UserFilterBy.IsActive)
                .WithMessage("Active status must be either 'true' or 'false'.");
        });
    }
}
