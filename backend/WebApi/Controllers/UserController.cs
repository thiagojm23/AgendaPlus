using AgendaPlus.Application.Commands;
using AgendaPlus.Application.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("AgendaPlus/[controller]/[action]")]
public class UserController(ILogger<UserController> logger, IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser(CreateUserRequestDto dto)
    {
        var command = new CreateUserCommand(dto.FirstName, dto.LastName, dto.Email, dto.Password, dto.ConfirmPassword);
        var result = await mediator.Send(command);

        if (result.IsFailure)
        {
        }

        return Created();
    }
}