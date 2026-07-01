using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Auth.Commands.RefreshToken;


public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;