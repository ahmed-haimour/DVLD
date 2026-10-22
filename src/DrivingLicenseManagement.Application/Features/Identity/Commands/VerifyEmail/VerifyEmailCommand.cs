using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string Code)
    : IRequest<Result>;
