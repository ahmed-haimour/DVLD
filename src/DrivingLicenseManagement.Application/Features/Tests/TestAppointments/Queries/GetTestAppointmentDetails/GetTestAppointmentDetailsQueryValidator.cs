using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointmentDetails;

public sealed class GetTestAppointmentDetailsQueryValidator : AbstractValidator<GetTestAppointmentDetailsQuery>
{
    public GetTestAppointmentDetailsQueryValidator()
    {
        RuleFor(query => query.TestAppointmentId)
            .NotEmpty().WithMessage("Test appointment Id is required.");
    }
}
