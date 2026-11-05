using DrivingLicenseManagement.Api.Contracts.Licenses;
using DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueInternationalLicense;
using DrivingLicenseManagement.Application.Features.Licenses.Commands.IssueLocalDrivingLicense;
using DrivingLicenseManagement.Application.Features.Licenses.Commands.RenewLicense;
using DrivingLicenseManagement.Application.Features.Licenses.Commands.ReplaceLicense;
using DrivingLicenseManagement.Application.Features.Licenses.Queries.GetInternationalLicensesByPersonId;
using DrivingLicenseManagement.Application.Features.Licenses.Queries.GetLocalLicensesByPersonId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/licenses")]
public sealed class LicensesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [EndpointName("IssueLocalDrivingLicense")]
    [EndpointSummary("Issue a local driving license")]
    [EndpointDescription("Issues a first-time local driving license for an application whose required tests have all been passed.")]
    [ProducesResponseType(typeof(IssueLocalDrivingLicenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IssueLocalDrivingLicense(IssueLocalDrivingLicenseRequest request, CancellationToken ct)
    {
        var command = new IssueLocalDrivingLicenseCommand(
            request.LocalDrivingLicenseApplicationId,
            request.Notes);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetDriverLicense",
            new { licenseId = result.Value },
            new IssueLocalDrivingLicenseResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpPost("{licenseId:guid}/renew")]
    [EndpointName("RenewLicense")]
    [EndpointSummary("Renew a license")]
    [EndpointDescription("Renews an expired license, deactivating it and issuing a new one in the same class.")]
    [ProducesResponseType(typeof(RenewLicenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RenewLicense(Guid licenseId, RenewLicenseRequest request, CancellationToken ct)
    {
        var command = new RenewLicenseCommand(
            licenseId,
            request.Notes);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetDriverLicense",
            new { licenseId = result.Value },
            new RenewLicenseResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpPost("{licenseId:guid}/replace")]
    [EndpointName("ReplaceLicense")]
    [EndpointSummary("Replace a license")]
    [EndpointDescription("Replaces a damaged or lost license, deactivating it and issuing a new one in the same class.")]
    [ProducesResponseType(typeof(ReplaceLicenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReplaceLicense(Guid licenseId, ReplaceLicenseRequest request, CancellationToken ct)
    {
        var command = new ReplaceLicenseCommand(
            licenseId,
            request.ReplaceReason,
            request.Notes);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetDriverLicense",
            new { licenseId = result.Value },
            new ReplaceLicenseResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpPost("{licenseId:guid}/international")]
    [EndpointName("IssueInternationalLicense")]
    [EndpointSummary("Issue an international license")]
    [EndpointDescription("Issues an international license based on an active, unexpired, non-detained ordinary local license.")]
    [ProducesResponseType(typeof(IssueInternationalLicenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IssueInternationalLicense(Guid licenseId, CancellationToken ct)
    {
        var result = await _sender.Send(new IssueInternationalLicenseCommand(licenseId), ct);

        return result.IsSuccess ? CreatedAtRoute("GetDriverInternationalLicense",
            new { internationalLicenseId = result.Value },
            new IssueInternationalLicenseResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpGet("by-person/{personId:guid}")]
    [EndpointName("GetLocalLicensesByPersonId")]
    [EndpointSummary("List a person's local licenses")]
    [EndpointDescription("Returns all local licenses issued to the person with the specified ID.")]
    [ProducesResponseType(typeof(IReadOnlyList<PersonLocalLicenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PersonLocalLicenseResponse>>> GetLocalLicensesByPersonId(Guid personId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetLocalLicensesByPersonIdQuery(personId), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpGet("international/by-person/{personId:guid}")]
    [EndpointName("GetInternationalLicensesByPersonId")]
    [EndpointSummary("List a person's international licenses")]
    [EndpointDescription("Returns all international licenses issued to the person with the specified ID.")]
    [ProducesResponseType(typeof(IReadOnlyList<PersonInternationalLicenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<PersonInternationalLicenseResponse>>> GetInternationalLicensesByPersonId(Guid personId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetInternationalLicensesByPersonIdQuery(personId), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }
}
