using AgendaPlus.Application.Commands.AvailabilityExceptions;
using AgendaPlus.Application.Queries.AvailabilityExceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("AgendaPlus/[controller]")]
public class AvailabilityExceptionController(
    ILogger<AvailabilityExceptionController> logger,
    IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateAvailabilityExceptionCommand command)
    {
        logger.LogInformation("Creating availability exception for resource ID: {ResourceId}", command.ResourceId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateAvailabilityExceptionCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        logger.LogInformation("Updating availability exception with ID: {Id}", id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        logger.LogInformation("Deleting availability exception with ID: {Id}", id);
        var command = new DeleteAvailabilityExceptionCommand(id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        logger.LogInformation("Getting availability exception with ID: {Id}", id);
        var query = new GetAvailabilityExceptionQuery(id);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult> List(
        [FromQuery] Guid? resourceId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        logger.LogInformation("Listing availability exceptions - Page: {Page}, PageSize: {PageSize}", pageNumber,
            pageSize);

        var query = new ListAvailabilityExceptionsQuery(resourceId, startDate, endDate, pageNumber, pageSize);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}