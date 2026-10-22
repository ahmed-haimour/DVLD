using DrivingLicenseManagement.Api.Contracts.TestTypes;
using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Features.TestTypes.Commands.UpdateTestType;
using DrivingLicenseManagement.Application.Features.TestTypes.Queries.GetTestTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/test-types")]
public sealed class TestTypesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [EndpointName("GetTestTypes")]
    [EndpointSummary("List test types")]
    [EndpointDescription("Returns all test types with their titles, descriptions and fees.")]
    [ProducesResponseType(typeof(IReadOnlyList<TestTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TestTypeResponse>>> GetTestTypes(CancellationToken ct)
    {
        var result = await _sender.Send(new GetTestTypesQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [EndpointName("UpdateTestType")]
    [EndpointSummary("Update a test type")]
    [EndpointDescription("Updates the title, description and fees of the test type with the specified ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTestType(Guid id, UpdateTestTypeRequest request, CancellationToken ct)
    {
        var command = new UpdateTestTypeCommand(
            id,
            request.TestTypeTitle,
            request.TestTypeDescription,
            request.TestTypeFees);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }
}
