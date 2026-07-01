namespace DrivingLicenseManagement.Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error PersonNationalNumberAlreadyExists = Error.Conflict(
        "Person.NationalNumberAlreadyExists",
        "A person with the same national number already exists.");

    public static readonly Error CountryNotFound = Error.NotFound(
        "Country.NotFound",
        "The selected nationality country was not found.");

    public static readonly Error PersonNotFound = Error.NotFound(
        "Person.NotFound",
        "The selected person was not found.");

    public static readonly Error PersonCannotBeDeleted = Error.Conflict(
        "Person.CannotBeDeleted",
        "The selected person cannot be deleted because they have related records.");

    public static readonly Error UserNotAuthenticated = Error.Unauthorized(
        "User.NotAuthenticated",
        "There is no authenticated user.");

        public static readonly Error RefreshTokenInvalid = Error.Unauthorized(
        "Refresh_Token_Invalid",
        "The refresh token is invalid, expired, or revoked.");
}
