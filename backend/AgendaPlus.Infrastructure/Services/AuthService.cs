using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;

namespace AgendaPlus.Infrastructure.Services;

public class AuthService(AuthTokenQueryBuilder authTokenQueryBuilder, IApplicationDbContext context, ITokenService tokenService) : IAuthService
{
    public async Task<Result> ValidateRefreshTokenAsync(string refreshToken)
    {
        var userId = tokenService.GetUserIdFromToken(refreshToken);
        if (userId == Guid.Empty)
            return Result.Failure("Invalid token structure.");

        var token = await authTokenQueryBuilder.GetByRefreshTokenAsync(refreshToken);

        if (token is null)
            return Result.Failure("Token not found.");

        if (token.ExpiresAt < DateTime.UtcNow)
            return Result.Failure("Token has expired.");

        if (token.UserId != userId)
        {
            token.UpdateRefreshToken(null);
            var secondToken = await authTokenQueryBuilder.GetByUserIdAsync(token.UserId);
            if (secondToken is not null)
            {
                secondToken.UpdateRefreshToken(null);
            }

            await context.SaveChangesAsync();
            return Result.Failure("Token does not belong to this user.");
        }

        return Result.Success();
    }
}