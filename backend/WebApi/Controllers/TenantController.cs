using AgendaPlus.Application.Commands.Tenants;
using AgendaPlus.Application.Queries.Tenants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("AgendaPlus/[controller]")]
public class TenantController(
    IMediator mediator,
    ILogger<TenantController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTenantCommand command)
    {
        logger.LogInformation("Creating tenant with name: {Name}", command.Name);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTenantCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        logger.LogInformation("Updating tenant with ID: {Id}", id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        logger.LogInformation("Deleting tenant with ID: {Id}", id);
        var command = new DeleteTenantCommand(id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        logger.LogInformation("Getting tenant with ID: {Id}", id);
        var query = new GetTenantQuery(id);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null)
    {
        logger.LogInformation("Listing tenants - Page: {Page}, PageSize: {PageSize}", pageNumber, pageSize);
        var query = new ListTenantsQuery(pageNumber, pageSize, isActive);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{id}/address")]
    public async Task<ActionResult> UpdateAddress(Guid id, [FromBody] UpdateTenantAddressCommand command)
    {
        if (id != command.TenantId)
            return BadRequest("ID mismatch");

        logger.LogInformation("Updating address for tenant ID: {Id}", id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}