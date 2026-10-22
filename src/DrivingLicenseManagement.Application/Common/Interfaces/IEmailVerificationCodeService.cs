namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface IEmailVerificationCodeService
{
    string GenerateCode();

    string HashCode(string code);

    bool VerifyCode(string code, string codeHash);
}