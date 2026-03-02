using AgendaPlus.Application.Commands.Resources;
using AgendaPlus.Application.Queries.Resources;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("AgendaPlus/[controller]")]
public class ResourceController(
    ILogger<ResourceController> logger,
    IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateResourceCommand command)
    {
        logger.LogInformation("Creating resource with name: {Name}", command.Name);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateResourceCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        logger.LogInformation("Updating resource with ID: {Id}", id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        logger.LogInformation("Deleting resource with ID: {Id}", id);
        var command = new DeleteResourceCommand(id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        logger.LogInformation("Getting resource with ID: {Id}", id);
        var query = new GetResourceQuery(id);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        [FromQuery] int? type = null)
    {
        logger.LogInformation("Listing resources - Page: {Page}, PageSize: {PageSize}", pageNumber, pageSize);

        var typeEnum = type.HasValue ? (ResourceType?)type.Value : null;
        var query = new ListResourcesQuery(pageNumber, pageSize, isActive, typeEnum);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}