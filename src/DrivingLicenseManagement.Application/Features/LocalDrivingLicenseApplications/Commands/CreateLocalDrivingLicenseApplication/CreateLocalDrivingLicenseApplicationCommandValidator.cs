using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CreateLocalDrivingLicenseApplication;

public sealed class CreateLocalDrivingLicenseApplicationCommandValidator
    : AbstractValidator<CreateLocalDrivingLicenseApplicationCommand>
{
    public CreateLocalDrivingLicenseApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicantPersonId)
            .NotEmpty()
            .WithMessage("Applicant is required.");

        RuleFor(x => x.LicenseClassId)
            .NotEmpty()
            .WithMessage("License class is required.");
    }
}
