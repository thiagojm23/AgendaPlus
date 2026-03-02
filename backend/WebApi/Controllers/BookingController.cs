using AgendaPlus.Application.Commands.Bookings;
using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Route("AgendaPlus/[controller]")]
public class BookingController(
    IMediator mediator,
    ILogger<BookingController> logger) : ControllerBase
{
    [HttpPost("check-availability")]
    [AllowAnonymous]
    public async Task<ActionResult> CheckAvailability([FromBody] CheckAvailabilityQuery query)
    {
        logger.LogInformation("Checking availability for Resource {ResourceId}", query.ResourceId);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("logged")]
    [Authorize]
    public async Task<ActionResult<Guid>> CreateLoggedBooking([FromBody] CreateBookingLoggedCommand command)
    {
        logger.LogInformation("Creating logged booking for Resource {ResourceId}", command.ResourceId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPost("guest")]
    [AllowAnonymous]
    public async Task<ActionResult<Guid>> CreateGuestBooking([FromBody] CreateBookingGuestCommand command)
    {
        logger.LogInformation("Creating guest booking for Resource {ResourceId}", command.ResourceId);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(SearchByCode), new { code = command.IdempotencyKey ?? result.Value.ToString() },
            result.Value);
    }

    [HttpPut("{id}/cancel")]
    [Authorize]
    public async Task<ActionResult> Cancel(Guid id)
    {
        logger.LogInformation("Cancelling booking with ID: {Id}", id);
        var command = new CancelBookingCommand(id);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetById(Guid id)
    {
        logger.LogInformation("Getting booking with ID: {Id}", id);
        var query = new GetBookingQuery(id);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> List(
        [FromQuery] Guid? resourceId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        logger.LogInformation("Listing bookings - Page: {Page}, PageSize: {PageSize}", pageNumber, pageSize);
        var query = new ListBookingsQuery(resourceId, startDate, endDate, status, pageNumber, pageSize);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult> ListMyBookings(
        [FromQuery] BookingStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        logger.LogInformation("Listing my bookings");
        var query = new ListMyBookingsQuery(status, pageNumber, pageSize);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("search/email/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult> SearchByEmail(string email)
    {
        logger.LogInformation("Searching bookings by email: {Email}", email);
        var query = new SearchBookingByEmailQuery(email);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("search/code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult> SearchByCode(string code)
    {
        logger.LogInformation("Searching booking by code: {Code}", code);
        var query = new SearchBookingByCodeQuery(code);
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}