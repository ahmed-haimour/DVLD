using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.ApplicationTypes.Commands.UpdateApplicationType;

public sealed class UpdateApplicationTypeCommandValidator : AbstractValidator<UpdateApplicationTypeCommand>
{
    public UpdateApplicationTypeCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Application type id is required.");
            
        RuleFor(x => x.ApplicationTypeTitle)
            .NotEmpty()
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Application type title is required.")
            .MaximumLength(150);

        RuleFor(command => command.ApplicationTypeFees)
            .GreaterThanOrEqualTo(0).WithMessage("Application type fees must not be negative.");
    }
}
