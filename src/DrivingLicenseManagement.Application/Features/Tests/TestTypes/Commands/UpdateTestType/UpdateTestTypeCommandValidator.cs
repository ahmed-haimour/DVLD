using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.TestTypes.Commands.UpdateTestType;

public sealed class UpdateTestTypeCommandValidator : AbstractValidator<UpdateTestTypeCommand>
{
    public UpdateTestTypeCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Test type id is required.");

        RuleFor(command => command.TestTypeTitle)
            .NotEmpty().WithMessage("Test type title is required.")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .MaximumLength(100).WithMessage("Test type title must not exceed 100 characters.");

        RuleFor(command => command.TestTypeDescription)
            .NotEmpty().WithMessage("Test type description is required.")
            .Must(description => !string.IsNullOrWhiteSpace(description))
            .MaximumLength(500).WithMessage("Test type description must not exceed 500 characters.");

        RuleFor(command => command.TestTypeFees)
            .GreaterThanOrEqualTo(0).WithMessage("Test type fees must not be negative.");
    }
}
