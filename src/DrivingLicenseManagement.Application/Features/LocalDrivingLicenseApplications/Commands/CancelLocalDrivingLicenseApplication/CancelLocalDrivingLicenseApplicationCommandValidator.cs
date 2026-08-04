using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CancelLocalDrivingLicenseApplication;

public sealed class CancelLocalDrivingLicenseApplicationCommandValidator
    : AbstractValidator<CancelLocalDrivingLicenseApplicationCommand>
{
    public CancelLocalDrivingLicenseApplicationCommandValidator()
    {
        RuleFor(x => x.LocalDrivingLicenseApplicationId)
            .NotEmpty()
            .WithMessage("Local driving license application is required.");
    }
}
