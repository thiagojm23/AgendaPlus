using AgendaPlus.Application.Queries.Users;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class GetUserQueryHandler(
    ILogger<GetUserQueryHandler> logger,
    UserQueryBuilder userQueryBuilder) : IRequestHandler<GetUserQuery, Result<User>>
{
    public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user with ID: {Id}", request.Id);

        var user = await userQueryBuilder
            .AsNoTracking()
            .WithUserTenants()
            .GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found with ID: {Id}", request.Id);
            return Result.Failure<User>("User not found");
        }

        logger.LogInformation("User found with ID: {Id}", request.Id);
        return Result.Success(user);
    }
}