using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.VerifyEmail;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email verification code is required.")
            .Matches(@"\A[0-9]{6}\z")
            .WithMessage("Email verification code must contain exactly 6 digits.");
    }
}
