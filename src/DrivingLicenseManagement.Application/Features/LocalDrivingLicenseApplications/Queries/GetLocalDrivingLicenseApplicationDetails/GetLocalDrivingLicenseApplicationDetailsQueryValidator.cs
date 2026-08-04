using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplicationDetails;

public sealed class GetLocalDrivingLicenseApplicationDetailsQueryValidator
    : AbstractValidator<GetLocalDrivingLicenseApplicationDetailsQuery>
{
    public GetLocalDrivingLicenseApplicationDetailsQueryValidator()
    {
        RuleFor(query => query.LocalDrivingLicenseApplicationId)
            .NotEmpty().WithMessage("Local driving license application is required.");
    }
}
