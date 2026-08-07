using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.ReplaceLicense;

public sealed class ReplaceLicenseCommandValidator : AbstractValidator<ReplaceLicenseCommand>
{
    public ReplaceLicenseCommandValidator()
    {
        RuleFor(command => command.LicenseId)
            .NotEmpty().WithMessage("License id is required.");

        RuleFor(command => command.ReplaceReason)
            .IsInEnum().WithMessage("Replacement reason must be a valid value.");

        RuleFor(command => command.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
