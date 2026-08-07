using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Commands.TakeTest;

public sealed class TakeTestCommandValidator : AbstractValidator<TakeTestCommand>
{
    public TakeTestCommandValidator()
    {
        RuleFor(command => command.TestAppointmentId)
            .NotEmpty().WithMessage("Test appointment Id is required.");

        RuleFor(command => command.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
