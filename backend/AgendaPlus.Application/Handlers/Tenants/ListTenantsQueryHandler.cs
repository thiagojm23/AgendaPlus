using AgendaPlus.Application.Queries.Tenants;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Tenants;

public class ListTenantsQueryHandler(
    ILogger<ListTenantsQueryHandler> logger,
    TenantQueryBuilder tenantQueryBuilder) : IRequestHandler<ListTenantsQuery, Result<List<Tenant>>>
{
    public async Task<Result<List<Tenant>>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing tenants - Page: {Page}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var queryBuilder = tenantQueryBuilder
            .AsNoTracking()
            .WithAdress();

        var query = queryBuilder.Query;

        if (request.IsActive.HasValue) query = query.Where(t => t.IsActive == request.IsActive.Value);

        var tenants = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} tenants", tenants.Count);
        return Result.Success(tenants);
    }
}