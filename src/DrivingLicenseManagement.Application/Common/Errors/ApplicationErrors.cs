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

    public static readonly Error PersonAlreadyHasUserAccount = Error.Conflict(
        "Person.AlreadyHasUserAccount",
        "The selected person already has a user account.");

    public static readonly Error UserNotAuthenticated = Error.Unauthorized(
        "User.NotAuthenticated",
        "There is no authenticated user.");

        public static readonly Error RefreshTokenInvalid = Error.Unauthorized(
        "Refresh_Token_Invalid",
        "The refresh token is invalid, expired, or revoked.");

        public static readonly Error PersonEmailMismatch = Error.Conflict(
            "Person.EmailMismatch",
            "The provided email does not match the person's email.");

    public static readonly Error ApplicationTypeNotFound = Error.NotFound(
        "ApplicationType.NotFound",
        "The selected application type was not found.");

    public static readonly Error LicenseClassNotFound = Error.NotFound(
        "LicenseClass.NotFound",
        "The selected license class was not found.");

    public static readonly Error ActiveLocalDrivingLicenseApplicationExists = Error.Conflict(
        "LocalDrivingLicenseApplication.ActiveApplicationExists",
        "The person already has an active local driving license application for this license class.");

    public static readonly Error ApplicantBelowMinimumAge = Error.Conflict(
        "LocalDrivingLicenseApplication.ApplicantBelowMinimumAge",
        "The applicant does not meet the minimum age required for the selected license class.");

    public static readonly Error LocalDrivingLicenseApplicationNotFound = Error.NotFound(
        "LocalDrivingLicenseApplication.NotFound",
        "The selected local driving license application was not found.");

    public static readonly Error LocalDrivingLicenseApplicationCannotBeCancelled = Error.Conflict(
        "LocalDrivingLicenseApplication.CannotBeCancelled",
        "Only a new local driving license application can be cancelled.");

    public static readonly Error TestTypeNotFound = Error.NotFound(
        "TestType.NotFound",
        "The selected test type was not found.");
}
