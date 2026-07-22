using DrivingLicenseManagement.Application.Common.Constants;
using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty()
            .WithMessage("Person is required.");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username may only contain letters, numbers, and underscores.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.")
            .Must(password => !string.IsNullOrEmpty(password) && password.Distinct().Count() > 1)
            .WithMessage("Password must not consist of a single repeated character.");

        RuleFor(x => x.Role)
            .Must(role => Roles.All.Contains(role))
            .WithMessage("Invalid role.");
    }
}
