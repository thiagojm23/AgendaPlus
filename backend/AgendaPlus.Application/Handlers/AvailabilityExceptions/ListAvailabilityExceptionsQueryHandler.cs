using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.Queries.AvailabilityExceptions;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AvailabilityException = AgendaPlus.Domain.Entities.AvailabilityExceptions;

namespace AgendaPlus.Application.Handlers.AvailabilityExceptions;

public class ListAvailabilityExceptionsQueryHandler(
    ILogger<ListAvailabilityExceptionsQueryHandler> logger,
    AvailabilityExceptionsQueryBuilder availabilityExceptionsQueryBuilder,
    ICurrentUserService currentUserService)
    : IRequestHandler<ListAvailabilityExceptionsQuery, Result<List<AvailabilityException>>>
{
    public async Task<Result<List<AvailabilityException>>> Handle(ListAvailabilityExceptionsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing availability exceptions - Page: {Page}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure<List<AvailabilityException>>("User has no associated tenant");
        }

        var query = availabilityExceptionsQueryBuilder.Query
            .AsNoTracking()
            .Where(ae => ae.TenantId == tenantId);

        if (request.ResourceId.HasValue && request.ResourceId.Value != Guid.Empty)
            query = query.Where(ae => ae.ResourceId == request.ResourceId.Value);

        if (request.StartDate.HasValue) 
            query = query.Where(ae => ae.StartBlockTime >= request.StartDate.Value);

        if (request.EndDate.HasValue) 
            query = query.Where(ae => ae.EndBlockTime <= request.EndDate.Value);

        var availabilityExceptions = await query
            .OrderBy(ae => ae.StartBlockTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} availability exceptions", availabilityExceptions.Count);
        return Result.Success(availabilityExceptions);
    }
}