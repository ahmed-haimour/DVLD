using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email)
    : IRequest<Result>;