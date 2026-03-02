using AgendaPlus.Application.Commands.Users;
using AgendaPlus.Application.Queries.Users;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Route("AgendaPlus/[controller]")]
public class UserController(ILogger<UserController> logger, IMediator mediator) : ControllerBase
{
    [HttpPost("create/owner")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateOwner([FromBody] CreateOwnerUserCommand command)
    {
        logger.LogInformation("Create owner user request received for email: {Email}", command.Email);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPost("create/consumer")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateConsumer([FromBody] CreateConsumerUserCommand command)
    {
        logger.LogInformation("Create consumer user request received for email: {Email}", command.Email);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    // [HttpPost("legacy")]
    // [AllowAnonymous]
    // public async Task<IActionResult> CreateUser(CreateUserRequestDto dto)
    // {
    //     logger.LogInformation("Create user request received for email: {Email}", dto.Email);
    //
    //     var command = new CreateUserCommand(dto.FirstName, dto.LastName, dto.Email, dto.Password, dto.ConfirmPassword);
    //     var result = await mediator.Send(command);
    //
    //     if (result.IsFailure) return BadRequest(result.Error);
    //
    //     return Created();
    // }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        logger.LogInformation("Updating user with ID: {Id}", id);

        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        logger.LogInformation("Deleting user with ID: {Id}", id);

        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        logger.LogInformation("Getting user with ID: {Id}", id);

        var query = new GetUserQuery(id);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? role = null,
        [FromQuery] bool? isActive = null)
    {
        logger.LogInformation("Listing users - Page: {Page}, PageSize: {PageSize}", pageNumber, pageSize);

        var roleEnum = role.HasValue ? (UserRole?)role.Value : null;
        var query = new ListUsersQuery(pageNumber, pageSize, roleEnum, isActive);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        logger.LogInformation("Getting current user");

        var query = new GetCurrentUserQuery();
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}