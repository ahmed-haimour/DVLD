using DrivingLicenseManagement.Api.Contracts.LicenseClasses;
using DrivingLicenseManagement.Application.Features.LicenseClasses.Queries.GetLicenseClasses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/license-classes")]
public sealed class LicenseClassesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [EndpointName("GetLicenseClasses")]
    [EndpointSummary("List license classes")]
    [EndpointDescription("Returns all license classes with their descriptions, minimum allowed age, validity length and fees.")]
    [ProducesResponseType(typeof(IReadOnlyList<LicenseClassResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<LicenseClassResponse>>> GetLicenseClasses(CancellationToken ct)
    {
        var result = await _sender.Send(new GetLicenseClassesQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }
}
