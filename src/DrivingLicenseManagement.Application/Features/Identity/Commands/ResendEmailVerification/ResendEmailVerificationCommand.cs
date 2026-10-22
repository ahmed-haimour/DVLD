using DrivingLicenseManagement.Application.Common.Models;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Identity.Commands.ResendEmailVerification;

public sealed record ResendEmailVerificationCommand(string Email) : IRequest<Result>;

