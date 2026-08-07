namespace DrivingLicenseManagement.Application.Common.Constants;

public static class TestTypeIds
{
    public static readonly Guid VisionTest = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid WrittenTest = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid PracticalStreetTest = Guid.Parse("33333333-3333-3333-3333-333333333333");

    // Index in this array defines the mandatory test order: Vision(0) → Written(1) → Practical(2).
    public static readonly Guid[] OrderedIds = [VisionTest, WrittenTest, PracticalStreetTest];
}
