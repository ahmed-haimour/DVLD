using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.ReleaseDetainedLicense;

public sealed class ReleaseDetainedLicenseCommandValidator : AbstractValidator<ReleaseDetainedLicenseCommand>
{
    public ReleaseDetainedLicenseCommandValidator()
    {
        RuleFor(command => command.DetainedLicenseId)
            .NotEmpty().WithMessage("Detained license id is required.");
    }
}
