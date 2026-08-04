using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Enums;
using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplications;

public sealed class GetLocalDrivingLicenseApplicationsQueryValidator
    : AbstractValidator<GetLocalDrivingLicenseApplicationsQuery>
{
    public GetLocalDrivingLicenseApplicationsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(query => query.FilterBy)
            .IsInEnum().WithMessage("Invalid filter field.");

        When(query => query.FilterBy != LocalDrivingLicenseApplicationFilterBy.None, () =>
        {
            RuleFor(query => query.FilterValue)
                .NotEmpty().WithMessage("A filter value is required when a filter field is selected.")
                .MaximumLength(100).WithMessage("Filter value must not exceed 100 characters.");
        });
    }
}
