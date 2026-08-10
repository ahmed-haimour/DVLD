namespace DrivingLicenseManagement.Application.Common.Constants;

public static class ApplicationTypeIds
{
    public static readonly Guid NewLocalDrivingLicense = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    public static readonly Guid RetakeTest = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");
    public static readonly Guid RenewLicense = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
    public static readonly Guid ReplacementForDamagedLicense = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");
    public static readonly Guid ReplacementForLostLicense = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005");
    public static readonly Guid NewInternationalLicense = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006");
    public static readonly Guid ReleaseDetainedDrivingLicense = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000007");
}
