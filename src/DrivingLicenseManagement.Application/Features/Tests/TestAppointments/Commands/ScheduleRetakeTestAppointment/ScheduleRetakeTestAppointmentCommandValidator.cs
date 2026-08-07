using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleRetakeTestAppointment;

public sealed class ScheduleRetakeTestAppointmentCommandValidator : AbstractValidator<ScheduleRetakeTestAppointmentCommand>
{
    public ScheduleRetakeTestAppointmentCommandValidator()
    {
        RuleFor(command => command.LocalDrivingLicenseApplicationId)
            .NotEmpty().WithMessage("Local driving license application is required.");

        RuleFor(command => command.TestTypeId)
            .NotEmpty().WithMessage("Test type is required.");

        RuleFor(command => command.AppointmentDate)
            .GreaterThanOrEqualTo(_ => DateTime.UtcNow.Date)
            .WithMessage("Appointment date cannot be in the past.");
    }
}
