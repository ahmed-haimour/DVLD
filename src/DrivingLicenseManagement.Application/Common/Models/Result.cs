using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;

namespace DrivingLicenseManagement.Application.Common.Models;

public class Result : IResult
{
    protected Result(bool isSuccess, List<Error>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;

    public List<Error>? Errors { get; }

    // Most failures carry a single error and call sites read it directly; expose the first
    // one so they don't have to index Errors.
    public Error? Error => Errors is { Count: > 0 } ? Errors[0] : null;

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, [error]);
    }

    public static Result Failure(List<Error> errors)
    {
        return new Result(false, errors);
    }
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, null)
    {
        _value = value;
    }

    private Result(List<Error> errors)
        : base(false, errors)
    {
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public new static Result<T> Failure(Error error)
    {
        return new Result<T>([error]);
    }

    public new static Result<T> Failure(List<Error> errors)
    {
        return new Result<T>(errors);
    }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return Failure(error);
    }
}
