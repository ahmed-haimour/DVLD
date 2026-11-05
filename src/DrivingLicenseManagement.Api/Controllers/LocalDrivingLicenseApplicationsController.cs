using DrivingLicenseManagement.Api.Contracts.LocalDrivingLicenseApplications;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CancelLocalDrivingLicenseApplication;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Commands.CreateLocalDrivingLicenseApplication;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplicationDetails;
using DrivingLicenseManagement.Application.Features.LocalDrivingLicenseApplications.Queries.GetLocalDrivingLicenseApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/local-driving-license-applications")]
public sealed class LocalDrivingLicenseApplicationsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [EndpointName("CreateLocalDrivingLicenseApplication")]
    [EndpointSummary("Create a local driving license application")]
    [EndpointDescription("Creates a new local driving license application for a person in the specified license class.")]
    [ProducesResponseType(typeof(CreateLocalDrivingLicenseApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateLocalDrivingLicenseApplication(CreateLocalDrivingLicenseApplicationRequest request, CancellationToken ct)
    {
        var command = new CreateLocalDrivingLicenseApplicationCommand(
            request.ApplicantPersonId,
            request.LicenseClassId);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetLocalDrivingLicenseApplicationDetails",
            new { id = result.Value },
            new CreateLocalDrivingLicenseApplicationResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpGet]
    [EndpointName("GetLocalDrivingLicenseApplications")]
    [EndpointSummary("List local driving license applications")]
    [EndpointDescription("Returns a paged, optionally filtered list of local driving license applications.")]
    [ProducesResponseType(typeof(GetLocalDrivingLicenseApplicationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetLocalDrivingLicenseApplicationsResponse>> GetLocalDrivingLicenseApplications([FromQuery] GetLocalDrivingLicenseApplicationsRequest request, CancellationToken ct)
    {
        var query = new GetLocalDrivingLicenseApplicationsQuery(
            request.FilterBy,
            request.FilterValue,
            request.PageNumber,
            request.PageSize);

        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpGet("{id:guid}")]
    [EndpointName("GetLocalDrivingLicenseApplicationDetails")]
    [EndpointSummary("Get a local driving license application")]
    [EndpointDescription("Returns the details of the local driving license application with the specified ID, including passed tests and applicant.")]
    [ProducesResponseType(typeof(LocalDrivingLicenseApplicationDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LocalDrivingLicenseApplicationDetailsResponse>> GetLocalDrivingLicenseApplicationDetails(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetLocalDrivingLicenseApplicationDetailsQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpPost("{id:guid}/cancel")]
    [EndpointName("CancelLocalDrivingLicenseApplication")]
    [EndpointSummary("Cancel a local driving license application")]
    [EndpointDescription("Cancels the local driving license application with the specified ID while its status is still New.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelLocalDrivingLicenseApplication(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new CancelLocalDrivingLicenseApplicationCommand(id), ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }
}
