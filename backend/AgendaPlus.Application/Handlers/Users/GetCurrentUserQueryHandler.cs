using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.Queries.Users;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class GetCurrentUserQueryHandler(
    ILogger<GetCurrentUserQueryHandler> logger,
    UserQueryBuilder userQueryBuilder,
    ICurrentUserService currentUserService) : IRequestHandler<GetCurrentUserQuery, Result<User>>
{
    public async Task<Result<User>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        logger.LogInformation("Getting current user with ID: {Id}", userId);

        var user = await userQueryBuilder
            .AsNoTracking()
            .WithUserTenants()
            .GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Current user not found with ID: {Id}", userId);
            return Result.Failure<User>("User not found");
        }

        logger.LogInformation("Current user found with ID: {Id}", userId);
        return Result.Success(user);
    }
}