using DrivingLicenseManagement.Api.Contracts.DetainedLicenses;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.DetainLicense;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Commands.ReleaseDetainedLicense;
using DrivingLicenseManagement.Application.Features.DetainedLicenses.Queries.GetDetainedLicenses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/detained-licenses")]
public sealed class DetainedLicensesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [EndpointName("DetainLicense")]
    [EndpointSummary("Detain a license")]
    [EndpointDescription("Detains an active license that is not already detained and records the fine fees.")]
    [ProducesResponseType(typeof(DetainLicenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DetainLicenseResponse>> DetainLicense(DetainLicenseRequest request, CancellationToken ct)
    {
        var command = new DetainLicenseCommand(
            request.LicenseId,
            request.FineFees);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(new DetainLicenseResponse(result.Value))
            : Problem(result.Errors!);
    }

    [HttpPost("{id:guid}/release")]
    [EndpointName("ReleaseDetainedLicense")]
    [EndpointSummary("Release a detained license")]
    [EndpointDescription("Releases the detained license with the specified ID by creating a release application.")]
    [ProducesResponseType(typeof(ReleaseDetainedLicenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReleaseDetainedLicenseResponse>> ReleaseDetainedLicense(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new ReleaseDetainedLicenseCommand(id), ct);

        return result.IsSuccess
            ? Ok(new ReleaseDetainedLicenseResponse(result.Value))
            : Problem(result.Errors!);
    }

    [HttpGet]
    [EndpointName("GetDetainedLicenses")]
    [EndpointSummary("List detained licenses")]
    [EndpointDescription("Returns a paged, optionally filtered list of detained licenses.")]
    [ProducesResponseType(typeof(GetDetainedLicensesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetDetainedLicensesResponse>> GetDetainedLicenses([FromQuery] GetDetainedLicensesRequest request, CancellationToken ct)
    {
        var query = new GetDetainedLicensesQuery(
            request.FilterBy,
            request.FilterValue,
            request.PageNumber,
            request.PageSize);

        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }
}
