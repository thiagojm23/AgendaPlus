using AgendaPlus.Application.Commands;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers;

public class ResetPasswordCommandHandler(
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext context,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing reset password request for token");

        var user = await userQueryBuilder
            .WithToken()
            .GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found");
            return Result.Failure<bool>($"User not found for id: {request.UserId}");
        }

        if (user.Token == null)
        {
            logger.LogWarning("Password reset token not found");
            return Result.Failure<bool>("Invalid or expired token");
        }

        if (!user.Token.IsPasswordResetTokenValid())
        {
            logger.LogWarning("Password reset token expired for user: {UserId}", user.Id);
            return Result.Failure<bool>("Invalid or expired token");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        user.UpdatePassword(passwordHash);
        user.Token.ClearPasswordResetToken();

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Password reset successfully for user: {UserId}", user.Id);

        return Result.Success(true);
    }
}