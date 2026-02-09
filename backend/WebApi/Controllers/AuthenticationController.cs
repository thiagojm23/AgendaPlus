using AgendaPlus.Application.Commands;
using AgendaPlus.Application.DTOs;
using AgendaPlus.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

[ApiController]
[Route("AgendaPlus/[controller]/[action]")]
public class AuthenticationController(
    ICurrentUserService currentUser,
    IMediator mediator,
    ILogger<AuthenticationController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> RefreshToken()
    {
        logger.LogInformation("Refresh token action called");

        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Refresh token is missing");

        var command = new RefreshTokenCommand(currentUser.UserId, refreshToken);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        HttpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/Authentication"
        });
        HttpContext.Response.Cookies.Append("accessToken", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        logger.LogInformation("Refresh token appended to response cookies for user ID: {UserId}", currentUser.UserId);

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        logger.LogInformation("Login action called for email: {Email}", dto.Email);

        var command = new LoginCommand(dto.Email, dto.Password);
        var result = await mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        HttpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/Authentication"
        });
        HttpContext.Response.Cookies.Append("accessToken", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        logger.LogInformation("Cookies saved for email: {Email}", dto.Email);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<bool>> ForgotPassword(ForgotPasswordRequestDto dto)
    {
        logger.LogInformation("Forgot password action called for email: {Email}", dto.Email);

        var command = new ForgotPasswordCommand(dto.Email);
        var response = await mediator.Send(command);

        return Ok(response);
    }
}