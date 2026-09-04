using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<Result>;
