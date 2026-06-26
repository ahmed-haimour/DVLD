using DrivingLicenseManagement.Domain.Enums;
using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.People.Commands.UpdatePerson;

public sealed class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Person is required.");

        RuleFor(x => x.NationalNumber)
            .NotEmpty()
            .WithMessage("National number is required.")
            .MaximumLength(20)
            .WithMessage("National number must not exceed 20 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(20)
            .WithMessage("First name must not exceed 20 characters.");

        RuleFor(x => x.SecondName)
            .NotEmpty()
            .WithMessage("Second name is required.")
            .MaximumLength(20)
            .WithMessage("Second name must not exceed 20 characters.");

        RuleFor(x => x.ThirdName)
            .MaximumLength(20)
            .WithMessage("Third name must not exceed 20 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(20)
            .WithMessage("Last name must not exceed 20 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required.")
            .LessThanOrEqualTo(DateTime.Today.AddYears(-18))
            .WithMessage("Person must be at least 18 years old.");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Gender must be a valid value.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(500)
            .WithMessage("Address must not exceed 500 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{7,15}$")
            .WithMessage("Phone number must be 7-15 digits and may start with +.");

        RuleFor(x => x.Email)
            .MaximumLength(50)
            .WithMessage("Email must not exceed 50 characters.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.NationalityCountryId)
            .NotEmpty()
            .WithMessage("Nationality country is required.");

        RuleFor(x => x.ImagePath)
            .MaximumLength(250)
            .WithMessage("Image path must not exceed 250 characters.");
    }
}
