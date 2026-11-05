using DrivingLicenseManagement.Api.Contracts.Drivers;
using DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDriverLicense;
using DrivingLicenseManagement.Application.Features.Drivers.Queries.GetDrivers;
using DrivingLicenseManagement.Application.Features.Drivers.Queries.GetInternationalLicenseDriver;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/drivers")]
public sealed class DriversController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [EndpointName("GetDrivers")]
    [EndpointSummary("List drivers")]
    [EndpointDescription("Returns a paged, optionally filtered list of drivers with their active license counts.")]
    [ProducesResponseType(typeof(GetDriversResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetDriversResponse>> GetDrivers([FromQuery] GetDriversRequest request, CancellationToken ct)
    {
        var query = new GetDriversQuery(
            request.FilterBy,
            request.FilterValue,
            request.PageNumber,
            request.PageSize);

        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpGet("licenses/{licenseId:guid}")]
    [EndpointName("GetDriverLicense")]
    [EndpointSummary("Get a driver's local license")]
    [EndpointDescription("Returns the local license with the specified ID together with its driver's personal details.")]
    [ProducesResponseType(typeof(DriverLicenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DriverLicenseResponse>> GetDriverLicense(Guid licenseId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetDriverLicenseQuery(licenseId), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpGet("international-licenses/{internationalLicenseId:guid}")]
    [EndpointName("GetDriverInternationalLicense")]
    [EndpointSummary("Get a driver's international license")]
    [EndpointDescription("Returns the international license with the specified ID together with its driver's personal details.")]
    [ProducesResponseType(typeof(DriverInternationalLicenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DriverInternationalLicenseResponse>> GetDriverInternationalLicense(Guid internationalLicenseId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetInternationalLicenseDriverQuery(internationalLicenseId), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }
}
