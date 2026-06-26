namespace DrivingLicenseManagement.Application.Common.Errors;

public readonly record struct Error
{
    private Error(string code, string description, ErrorKind type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorKind Type { get; }

    public static Error Create(string code, string description, ErrorKind type)
    {
        return new Error(code, description, type);
    }

    public static Error Failure(string code = nameof(Failure), string description = "General failure.")
    {
        return Create(code, description, ErrorKind.Failure);
    }

    public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected error.")
    {
        return Create(code, description, ErrorKind.Unexpected);
    }

    public static Error Validation(string code = nameof(Validation), string description = "Validation error.")
    {
        return Create(code, description, ErrorKind.Validation);
    }

    public static Error Conflict(string code = nameof(Conflict), string description = "Conflict error.")
    {
        return Create(code, description, ErrorKind.Conflict);
    }

    public static Error NotFound(string code = nameof(NotFound), string description = "Not found error.")
    {
        return Create(code, description, ErrorKind.NotFound);
    }

    public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized error.")
    {
        return Create(code, description, ErrorKind.Unauthorized);
    }

    public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden error.")
    {
        return Create(code, description, ErrorKind.Forbidden);
    }
}
