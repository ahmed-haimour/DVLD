using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.DetainLicense;

public sealed class DetainLicenseCommandValidator : AbstractValidator<DetainLicenseCommand>
{
    public DetainLicenseCommandValidator()
    {
        RuleFor(command => command.LicenseId)
            .NotEmpty().WithMessage("License id is required.");

        RuleFor(command => command.FineFees)
            .GreaterThan(0).WithMessage("Fine fees must be greater than 0.");
    }
}
