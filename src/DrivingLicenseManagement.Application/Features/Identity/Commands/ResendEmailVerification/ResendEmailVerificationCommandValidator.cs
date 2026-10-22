using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ResendEmailVerification;

public sealed class ResendEmailVerificationCommandValidator
    : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");
    }
}