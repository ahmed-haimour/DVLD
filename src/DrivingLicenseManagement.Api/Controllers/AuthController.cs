using DrivingLicenseManagement.Application.Features.Auth.Commands.Login;
using DrivingLicenseManagement.Application.Features.Auth.Commands.Logout;
using DrivingLicenseManagement.Application.Features.Auth.Commands.RefreshToken;
using DrivingLicenseManagement.Application.Features.Auth.Dtos;
using DrivingLicenseManagement.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DrivingLicenseManagement.Api.Controllers;

[Route("api/auth")]
[EnableRateLimiting("SlidingWindow")]
public sealed class AuthController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [AllowAnonymous]
    [HttpPost("login")]
    [EndpointName("Login")]
    [EndpointSummary("Authenticate a user")]
    [EndpointDescription("Validates user credentials and returns an access and refresh token pair.")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType( typeof(ProblemDetails),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Errors!);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [EndpointName("Logout")]
    [EndpointSummary("Revoke a refresh token")]
    [EndpointDescription("Revokes the supplied refresh token.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [EndpointName("RefreshToken")]
    [EndpointSummary("Exchange a refresh token for a new token pair")]
    [EndpointDescription("Exchanges a valid refresh token for a new access and refresh token pair.")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Errors!);
    }

    [Authorize]
    [HttpGet("me")]
    [EndpointName("GetCurrentUser")]
    [EndpointSummary("Get the authenticated user's profile")]
    [EndpointDescription("Returns the currently authenticated user's account details.")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var result = await _sender.Send(new GetCurrentUserQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Errors!);
    }
}
