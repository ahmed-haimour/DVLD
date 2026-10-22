using DrivingLicenseManagement.Api.Contracts.Tests;
using DrivingLicenseManagement.Application.Features.Tests.Tests.Commands.TakeTest;
using DrivingLicenseManagement.Application.Features.Tests.Tests.Queries.GetTestDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/tests")]
public sealed class TestsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [EndpointName("TakeTest")]
    [EndpointSummary("Record a test result")]
    [EndpointDescription("Records the result of the test taken for the specified test appointment and locks the appointment.")]
    [ProducesResponseType(typeof(TakeTestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TakeTest(TakeTestRequest request, CancellationToken ct)
    {
        var command = new TakeTestCommand(
            request.TestAppointmentId,
            request.TestResult,
            request.Notes);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetTestDetails",
            new { id = result.Value },
            new TakeTestResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpGet("{id:guid}")]
    [EndpointName("GetTestDetails")]
    [EndpointSummary("Get test details")]
    [EndpointDescription("Returns the details of the test with the specified ID.")]
    [ProducesResponseType(typeof(TestDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TestDetailsResponse>> GetTestDetails(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetTestDetailsQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }
}
