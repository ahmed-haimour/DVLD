using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.UpdateTestAppointmentDate;

public sealed class UpdateTestAppointmentDateCommandValidator : AbstractValidator<UpdateTestAppointmentDateCommand>
{
    public UpdateTestAppointmentDateCommandValidator()
    {
        RuleFor(command => command.TestAppointmentId)
            .NotEmpty().WithMessage("Test appointment is required.");

        RuleFor(command => command.AppointmentDate)
            .GreaterThanOrEqualTo(_ => DateTime.UtcNow.Date)
            .WithMessage("Appointment date cannot be in the past.");
    }
}
