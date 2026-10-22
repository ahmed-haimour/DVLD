using DrivingLicenseManagement.Api.Contracts.Identity;
using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Features.Identity.Commands.ChangePassword;
using DrivingLicenseManagement.Application.Features.Identity.Commands.CreateUser;
using DrivingLicenseManagement.Application.Features.Identity.Commands.ForgotPassword;
using DrivingLicenseManagement.Application.Features.Identity.Commands.ResendEmailVerification;
using DrivingLicenseManagement.Application.Features.Identity.Commands.ResetPassword;
using DrivingLicenseManagement.Application.Features.Identity.Commands.VerifyEmail;
using DrivingLicenseManagement.Application.Features.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DrivingLicenseManagement.Api.Controllers;

[Route("api/identity")]
[EnableRateLimiting("SlidingWindow")]
public sealed class IdentityController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("users")]
    [EndpointName("CreateUser")]
    [EndpointSummary("Create a user account for an existing person")]
    [EndpointDescription("Creates a user account for a registered person and assigns it the requested role.")]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken ct)
    {
        var command = new CreateUserCommand(
            request.PersonId,
            request.UserName,
            request.Email,
            request.Password,
            request.Role);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, new CreateUserResponse(result.Value))
            : Problem(result.Errors!);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("users")]
    [EndpointName("GetUsers")]
    [EndpointSummary("List user accounts")]
    [EndpointDescription("Returns a paged, optionally filtered list of user accounts.")]
    [ProducesResponseType(typeof(GetUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request, CancellationToken ct)
    {
        var query = new GetAllUsersQuery(
            request.FilterBy,
            request.FilterValue,
            request.PageNumber,
            request.PageSize);

        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [Authorize]
    [HttpPut("change-password")]
    [EndpointName("ChangePassword")]
    [EndpointSummary("Change the authenticated user's password")]
    [EndpointDescription("Changes the current user's password and revokes every active refresh token.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        var command = new ChangePasswordCommand(
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmPassword);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [EndpointName("ForgotPassword")]
    [EndpointSummary("Request a password reset")]
    [EndpointDescription("Sends a password reset link when the email belongs to an existing account.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new ForgotPasswordCommand(request.Email), ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [EndpointName("ResetPassword")]
    [EndpointSummary("Reset a password")]
    [EndpointDescription("Resets an account password using the token sent to the account email address.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken ct)
    {
        var command = new ResetPasswordCommand(
            request.Email,
            request.Token,
            request.NewPassword,
            request.ConfirmPassword);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }


    [EnableRateLimiting("EmailVerification")]
    [AllowAnonymous]
    [HttpPost("verify-email")]
    [EndpointName("VerifyEmail")]
    [EndpointSummary("Verify an email address")]
    [EndpointDescription("Verifies an account email address using the six-digit verification code sent to the account email.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request, CancellationToken ct)
    {
        var command = new VerifyEmailCommand(
            request.Email,
            request.Code);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }


    [EnableRateLimiting("EmailVerificationResend")]
    [AllowAnonymous]
    [HttpPost("resend-email-verification")]
    [EndpointName("ResendEmailVerification")]
    [EndpointSummary("Resend email verification code")]
    [EndpointDescription("Generates and sends a new six-digit email verification code for an unverified account.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResendEmailVerification(
        ResendEmailVerificationRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new ResendEmailVerificationCommand(request.Email), ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }
}
