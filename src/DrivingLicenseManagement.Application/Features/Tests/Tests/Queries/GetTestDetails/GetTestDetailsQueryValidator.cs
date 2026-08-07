using FluentValidation;

namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Queries.GetTestDetails;

public sealed class GetTestDetailsQueryValidator : AbstractValidator<GetTestDetailsQuery>
{
    public GetTestDetailsQueryValidator()
    {
        RuleFor(query => query.TestId)
            .NotEmpty().WithMessage("Test Id is required.");
    }
}
