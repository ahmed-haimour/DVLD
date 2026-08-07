using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointments;

public sealed class GetTestAppointmentsQueryValidator : AbstractValidator<GetTestAppointmentsQuery>
{
    public GetTestAppointmentsQueryValidator()
    {
        RuleFor(query => query.LocalDrivingLicenseApplicationId)
            .NotEmpty().WithMessage("Local driving license application id is required.");

        RuleFor(query => query.TestTypeId)
            .NotEmpty().WithMessage("Test type id is required.");
    }
}
