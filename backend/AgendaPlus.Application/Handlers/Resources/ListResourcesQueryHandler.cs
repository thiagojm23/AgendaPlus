using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.Queries.Resources;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Resources;

public class ListResourcesQueryHandler(
    ILogger<ListResourcesQueryHandler> logger,
    ResourceQueryBuilder resourceQueryBuilder,
    ICurrentUserService currentUserService) : IRequestHandler<ListResourcesQuery, Result<List<Resource>>>
{
    public async Task<Result<List<Resource>>> Handle(ListResourcesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing resources - Page: {Page}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure<List<Resource>>("User has no associated tenant");
        }

        var query = resourceQueryBuilder.Query
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId);

        if (request.IsActive.HasValue)
            query = query.Where(r => r.IsActive == request.IsActive.Value);

        if (request.Type.HasValue)
            query = query.Where(r => r.ResourceType == request.Type.Value);

        var resources = await query
            .OrderBy(r => r.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} resources", resources.Count);
        return Result.Success(resources);
    }
}