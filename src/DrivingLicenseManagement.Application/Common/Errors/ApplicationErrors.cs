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

    public static readonly Error LocalDrivingLicenseApplicationNotActive = Error.Conflict(
        "LocalDrivingLicenseApplication.NotActive",
        "The local driving license application is not active.");

    public static readonly Error TestAppointmentNotFound = Error.NotFound(
        "TestAppointment.NotFound",
        "The selected test appointment was not found.");

    public static readonly Error TestAppointmentLocked = Error.Conflict(
        "TestAppointment.Locked",
        "This test appointment is locked and can no longer be modified.");

    public static readonly Error ActiveTestAppointmentExists = Error.Conflict(
        "TestAppointment.ActiveAppointmentExists",
        "There is already an active appointment for this test type.");

    public static readonly Error PreviousTestNotPassed = Error.Conflict(
        "TestAppointment.PreviousTestNotPassed",
        "The previous test must be passed before scheduling this test.");

    public static readonly Error TestTypeAlreadyPassed = Error.Conflict(
        "TestAppointment.TestTypeAlreadyPassed",
        "The applicant has already passed this test type.");

    public static readonly Error RetakeRequired = Error.Conflict(
        "TestAppointment.RetakeRequired",
        "A previous attempt for this test type already exists; schedule a retake instead.");

    public static readonly Error NoFailedTestToRetake = Error.Conflict(
        "TestAppointment.NoFailedTestToRetake",
        "There is no failed attempt for this test type to retake.");

    public static readonly Error TestNotFound = Error.NotFound(
        "Test.NotFound",
        "The selected test was not found.");

    public static readonly Error ActiveLicenseAlreadyExists = Error.Conflict(
        "Application.ActiveLicenseAlreadyExists",
        "The applicant already has an active license for this license class.");

    public static readonly Error RequiredTestsNotPassed = Error.Conflict(
        "License.RequiredTestsNotPassed",
        "The applicant has not passed all required tests.");

    public static readonly Error LicenseExpired = Error.Conflict(
        "License.Expired",
        "The license is expired.");

    public static readonly Error LicenseNotEligibleForInternational = Error.Conflict(
        "License.NotEligibleForInternational",
        "Only an ordinary (Class 3) driving license is eligible for an international license.");

    public static readonly Error ActiveInternationalLicenseExists = Error.Conflict(
        "InternationalLicense.ActiveLicenseExists",
        "The driver already has an active international license.");

    public static readonly Error LicenseNotFound = Error.NotFound(
        "License.NotFound",
        "The selected license was not found.");

    public static readonly Error LicenseNotActive = Error.Conflict(
        "License.NotActive",
        "The license is not active.");

    public static readonly Error LicenseNotExpired = Error.Conflict(
        "License.NotExpired",
        "Cannot renew a license that has not yet expired.");
}
