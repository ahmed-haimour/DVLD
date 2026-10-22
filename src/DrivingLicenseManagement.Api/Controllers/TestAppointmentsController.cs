using DrivingLicenseManagement.Api.Contracts.TestAppointments;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleRetakeTestAppointment;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.ScheduleTestAppointment;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Commands.UpdateTestAppointmentDate;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointmentDetails;
using DrivingLicenseManagement.Application.Features.Tests.TestAppointments.Queries.GetTestAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/test-appointments")]
public sealed class TestAppointmentsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [EndpointName("ScheduleTestAppointment")]
    [EndpointSummary("Schedule a test appointment")]
    [EndpointDescription("Schedules the first attempt of a test type for a local driving license application.")]
    [ProducesResponseType(typeof(ScheduleTestAppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ScheduleTestAppointment(ScheduleTestAppointmentRequest request, CancellationToken ct)
    {
        var command = new ScheduleTestAppointmentCommand(
            request.LocalDrivingLicenseApplicationId,
            request.TestTypeId,
            request.AppointmentDate);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetTestAppointmentDetails",
            new { id = result.Value },
            new ScheduleTestAppointmentResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpPost("retake")]
    [EndpointName("ScheduleRetakeTestAppointment")]
    [EndpointSummary("Schedule a retake test appointment")]
    [EndpointDescription("Schedules a retake of a previously failed test type, creating a retake test application.")]
    [ProducesResponseType(typeof(ScheduleTestAppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ScheduleRetakeTestAppointment(ScheduleTestAppointmentRequest request, CancellationToken ct)
    {
        var command = new ScheduleRetakeTestAppointmentCommand(
            request.LocalDrivingLicenseApplicationId,
            request.TestTypeId,
            request.AppointmentDate);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetTestAppointmentDetails",
            new { id = result.Value },
            new ScheduleTestAppointmentResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpGet]
    [EndpointName("GetTestAppointments")]
    [EndpointSummary("List test appointments")]
    [EndpointDescription("Returns the test appointments of a local driving license application for the specified test type.")]
    [ProducesResponseType(typeof(IReadOnlyList<TestAppointmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TestAppointmentResponse>>> GetTestAppointments(
        [FromQuery] GetTestAppointmentsRequest request,
        CancellationToken ct)
    {
        var query = new GetTestAppointmentsQuery(
            request.LocalDrivingLicenseApplicationId,
            request.TestTypeId);

        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpGet("{id:guid}")]
    [EndpointName("GetTestAppointmentDetails")]
    [EndpointSummary("Get test appointment details")]
    [EndpointDescription("Returns the details of the test appointment with the specified ID, including fees and test result.")]
    [ProducesResponseType(typeof(TestAppointmentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TestAppointmentDetailsResponse>> GetTestAppointmentDetails(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetTestAppointmentDetailsQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpPut("{id:guid}/appointment-date")]
    [EndpointName("UpdateTestAppointmentDate")]
    [EndpointSummary("Reschedule a test appointment")]
    [EndpointDescription("Changes the date of the test appointment with the specified ID while it is not locked.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTestAppointmentDate(
        Guid id,
        UpdateTestAppointmentDateRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new UpdateTestAppointmentDateCommand(id, request.AppointmentDate), ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }
}
