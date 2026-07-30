using System.Reflection;
using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace DrivingLicenseManagement.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse>
(IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
        where TResponse : IResult
{
     private readonly IValidator<TRequest>? _validator = validator;

    // Result.Failure(List<Error>) and Result<T>.Failure(List<Error>) are both static, and the
    // generic one hides the base with `new`, so DeclaredOnly binds the right one for either
    // TResponse instead of throwing AmbiguousMatchException.
    private static readonly MethodInfo FailureFactory = typeof(TResponse).GetMethod(
        nameof(Result.Failure),
        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
        [typeof(List<Error>)])!;

      public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, ct);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

        return (TResponse)FailureFactory.Invoke(null, [errors])!;
    }
}
