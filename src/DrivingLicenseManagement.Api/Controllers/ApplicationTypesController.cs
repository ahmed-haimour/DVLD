using DrivingLicenseManagement.Api.Contracts.ApplicationTypes;
using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Features.ApplicationTypes.Commands.UpdateApplicationType;
using DrivingLicenseManagement.Application.Features.ApplicationTypes.Queries.GetApplicationTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/application-types")]
public sealed class ApplicationTypesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [EndpointName("GetApplicationTypes")]
    [EndpointSummary("List application types")]
    [EndpointDescription("Returns all application types with their titles and fees.")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ApplicationTypeResponse>>> GetApplicationTypes(CancellationToken ct)
    {
        var result = await _sender.Send(new GetApplicationTypesQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [EndpointName("UpdateApplicationType")]
    [EndpointSummary("Update an application type")]
    [EndpointDescription("Updates the title and fees of the application type with the specified ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateApplicationType(Guid id, UpdateApplicationTypeRequest request, CancellationToken ct)
    {
        var command = new UpdateApplicationTypeCommand(
            id,
            request.ApplicationTypeTitle,
            request.ApplicationTypeFees);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }
}
