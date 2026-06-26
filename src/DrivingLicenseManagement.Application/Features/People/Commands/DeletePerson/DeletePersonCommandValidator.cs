using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.People.Commands.DeletePerson;

public sealed class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Person is required.");
    }
}
