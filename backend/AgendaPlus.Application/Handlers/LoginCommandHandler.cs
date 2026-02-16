using AgendaPlus.Application.Commands;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.DTOs.Responses;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers;

public class LoginCommandHandler(
    ITokenService tokenService,
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext context,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userQueryBuilder.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || BCrypt.Net.BCrypt.Verify(user.PasswordHash, request.Password))
        {
            logger.LogInformation("Failed to validate credentials for email: {Email}", request.Email);
            return Result.Failure<AuthResponseDto>("Invalid credentials", new UnauthorizedAccessException());
        }

        var accessToken = await tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(user);

        user.SetAuthToken(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully logged in: {Email}", request.Email);

        return Result.Success(new AuthResponseDto(accessToken, refreshToken));
    }
}