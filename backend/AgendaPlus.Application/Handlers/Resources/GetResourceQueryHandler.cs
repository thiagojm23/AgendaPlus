using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.Queries.Resources;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Resources;

public class GetResourceQueryHandler(
    ILogger<GetResourceQueryHandler> logger,
    ResourceQueryBuilder resourceQueryBuilder,
    ICurrentUserService currentUserService) : IRequestHandler<GetResourceQuery, Result<Resource>>
{
    public async Task<Result<Resource>> Handle(GetResourceQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting resource with ID: {Id}", request.Id);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure<Resource>("User has no associated tenant");
        }

        var resource = await resourceQueryBuilder
            .AsNoTracking()
            .GetByIdAsync(request.Id, cancellationToken);

        if (resource == null || resource.TenantId != tenantId)
        {
            logger.LogWarning("Resource not found with ID: {Id}", request.Id);
            return Result.Failure<Resource>("Resource not found");
        }

        logger.LogInformation("Resource found with ID: {Id}", request.Id);
        return Result.Success(resource);
    }
}