using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull().NotEmpty()
            .WithErrorCode("Refresh_Token_Null_Or_Empty")
            .WithMessage("Refresh token cannot be null or empty.");
    }
}
