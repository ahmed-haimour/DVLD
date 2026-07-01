using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using DrivingLicenseManagement.Application.Features.Identity;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<Result<TokenResponse>>;