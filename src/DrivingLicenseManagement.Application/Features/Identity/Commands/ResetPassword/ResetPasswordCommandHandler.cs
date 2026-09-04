using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using System.Buffers.Text;
using System.Text;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        string token;

        try
        {
            token = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(request.Token));
        }
        catch (FormatException)
        {
            // Keep malformed transport tokens indistinguishable from invalid Identity tokens.
            token = string.Empty;
        }

        return await _identityService.ResetPasswordAsync(
            request.Email,
            token,
            request.NewPassword,
            ct);
    }
}
