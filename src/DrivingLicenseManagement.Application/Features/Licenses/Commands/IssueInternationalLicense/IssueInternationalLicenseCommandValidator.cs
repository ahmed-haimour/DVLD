using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueInternationalLicense;

public sealed class IssueInternationalLicenseCommandValidator : AbstractValidator<IssueInternationalLicenseCommand>
{
    public IssueInternationalLicenseCommandValidator()
    {
        RuleFor(command => command.LicenseId)
            .NotEmpty().WithMessage("License id is required.");
    }
}
