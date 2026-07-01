using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull().NotEmpty()
            .WithErrorCode("Refresh_Token_Null_Or_Empty")
            .WithMessage("Refresh token cannot be null or empty.");
    }
}
