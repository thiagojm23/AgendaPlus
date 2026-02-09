using AgendaPlus.Application.Commands;
using AgendaPlus.Application.DTOs;
using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers;

public class RefreshTokenCommandHandler(
    IAuthService authService,
    ITokenService tokenService,
    ITokenRepository tokenRepository,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Refresh token handler started for user ID: {UserId}", request.UserId);

        var validationResult = await authService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (validationResult.IsFailure)
        {
            logger.LogWarning("Refresh token validation failed for user ID: {UserId}. Error: {Error}", request.UserId,
                validationResult.Error);
            return Result.Failure<AuthResponseDto>(validationResult.Error);
        }

        var token = (await tokenRepository.GetByRefreshTokenAsync(request.RefreshToken, token => token.User))!;

        logger.LogInformation("Generating new tokens for user ID: {UserId}", token.User.Id);

        var newAccessToken = await tokenService.GenerateAccessTokenAsync(token.User);
        var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(token.User);

        token.UpdateRefreshToken(newRefreshToken);

        await tokenRepository.UpdateAsync(token);

        logger.LogInformation("Refresh token successfully updated for user ID: {UserId}", token.User.Id);

        return Result.Success(new AuthResponseDto(newAccessToken, newRefreshToken));
    }
}