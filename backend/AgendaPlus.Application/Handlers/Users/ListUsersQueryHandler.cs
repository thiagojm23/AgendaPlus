using AgendaPlus.Application.Queries.Users;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class ListUsersQueryHandler(
    ILogger<ListUsersQueryHandler> logger,
    UserQueryBuilder userQueryBuilder) : IRequestHandler<ListUsersQuery, Result<List<User>>>
{
    public async Task<Result<List<User>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing users - Page: {Page}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var queryBuilder = userQueryBuilder
            .AsNoTracking()
            .WithUserTenants();

        var query = queryBuilder.Query;

        if (request.Role.HasValue) query = query.Where(u => u.Role == request.Role.Value);

        if (request.IsActive.HasValue) query = query.Where(u => u.IsActive == request.IsActive.Value);

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} users", users.Count);
        return Result.Success(users);
    }
}