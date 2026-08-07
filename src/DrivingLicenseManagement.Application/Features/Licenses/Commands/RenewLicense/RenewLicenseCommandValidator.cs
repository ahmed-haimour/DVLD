using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.RenewLicense;

public sealed class RenewLicenseCommandValidator : AbstractValidator<RenewLicenseCommand>
{
    public RenewLicenseCommandValidator()
    {
        RuleFor(command => command.LicenseId)
            .NotEmpty().WithMessage("License id is required.");

        RuleFor(command => command.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
