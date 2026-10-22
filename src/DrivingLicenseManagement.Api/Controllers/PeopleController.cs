using DrivingLicenseManagement.Api.Contracts.People;
using DrivingLicenseManagement.Application.Common.Constants;
using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.People.Commands.CreatePerson;
using DrivingLicenseManagement.Application.Features.People.Commands.DeletePerson;
using DrivingLicenseManagement.Application.Features.People.Commands.UpdatePerson;
using DrivingLicenseManagement.Application.Features.People.Dtos;
using DrivingLicenseManagement.Application.Features.People.Queries.GetPeople;
using DrivingLicenseManagement.Application.Features.People.Queries.GetPersonById;
using DrivingLicenseManagement.Application.Features.People.Queries.GetPersonByNationalNo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrivingLicenseManagement.Api.Controllers;

[Authorize]
[Route("api/people")]
public sealed class PeopleController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [EndpointName("CreatePerson")]
    [EndpointSummary("Create a person")]
    [EndpointDescription("Registers a person with their personal details and national number.")]
    [ProducesResponseType(typeof(CreatePersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePerson(PersonRequest request, CancellationToken ct)
    {
        var command = new CreatePersonCommand(
            request.NationalNumber,
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.Address,
            request.Phone,
            request.Email,
            request.NationalityCountryId,
            request.ImagePath);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? CreatedAtRoute("GetPersonById",
            new { id = result.Value },
            new CreatePersonResponse(result.Value))
        : Problem(result.Errors!);
    }

    [HttpGet]
    [EndpointName("GetPeople")]
    [EndpointSummary("List people")]
    [EndpointDescription("Returns a paged, optionally filtered list of people.")]
    [ProducesResponseType(typeof(GetPeopleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetPeopleResponse>> GetPeople([FromQuery] GetPeopleRequest request, CancellationToken ct)
    {
        var query = new GetPeopleQuery(
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
    [EndpointName("GetPersonById")]
    [EndpointSummary("Get a person by ID")]
    [EndpointDescription("Returns the personal details of the person with the specified ID.")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PersonResponse>> GetPersonById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetPersonByIdQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }

    [HttpGet("by-national-no/{nationalNo}")]
    [EndpointName("GetPersonByNationalNo")]
    [EndpointSummary("Get a person by national number")]
    [EndpointDescription("Returns the personal details of the person with the specified national number.")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponse>> GetPersonByNationalNo(string nationalNo, CancellationToken ct)
    {
        var result = await _sender.Send(new GetPersonByNationalNoQuery(nationalNo), ct);

        return result.IsSuccess
            ? Ok(result.Value.ToResponse())
            : Problem(result.Errors!);
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [EndpointName("UpdatePerson")]
    [EndpointSummary("Update a person")]
    [EndpointDescription("Updates the personal details of the person with the specified ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePerson(Guid id, PersonRequest request, CancellationToken ct)
    {
        var command = new UpdatePersonCommand(
            id,
            request.NationalNumber,
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.Address,
            request.Phone,
            request.Email,
            request.NationalityCountryId,
            request.ImagePath);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [EndpointName("DeletePerson")]
    [EndpointSummary("Delete a person")]
    [EndpointDescription("Deletes the person with the specified ID when no related records exist.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeletePerson(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeletePersonCommand(id), ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors!);
    }
}
