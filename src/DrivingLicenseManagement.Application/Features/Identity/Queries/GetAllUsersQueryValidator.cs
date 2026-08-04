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

            // Type-specific checks (GUID / bool) are intentionally omitted: search screens
            // return empty results for an unparseable filter value rather than a validation error.
        });
    }
}
