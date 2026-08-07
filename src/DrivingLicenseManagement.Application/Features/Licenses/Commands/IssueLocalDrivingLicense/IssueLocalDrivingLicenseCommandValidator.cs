using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueLocalDrivingLicense;

public sealed class IssueLocalDrivingLicenseCommandValidator : AbstractValidator<IssueLocalDrivingLicenseCommand>
{
    public IssueLocalDrivingLicenseCommandValidator()
    {
        RuleFor(command => command.LocalDrivingLicenseApplicationId)
            .NotEmpty().WithMessage("Local driving license application id is required.");

        RuleFor(command => command.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
