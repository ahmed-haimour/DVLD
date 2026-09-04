using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Password reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(6)
            .WithMessage("New password must be at least 6 characters.")
            .Must(password => !string.IsNullOrEmpty(password) && password.Distinct().Count() > 1)
            .WithMessage("New password must not consist of a single repeated character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Password confirmation is required.")
            .Equal(x => x.NewPassword)
            .WithMessage("Password confirmation does not match the new password.");
    }
}
