using AgendaPlus.Application.Queries.Tenants;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Tenants;

public class GetTenantQueryHandler(
    ILogger<GetTenantQueryHandler> logger,
    TenantQueryBuilder tenantQueryBuilder) : IRequestHandler<GetTenantQuery, Result<Tenant>>
{
    public async Task<Result<Tenant>> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tenant with ID: {Id}", request.Id);

        var tenant = await tenantQueryBuilder
            .AsNoTracking()
            .WithAdress()
            .GetByIdAsync(request.Id, cancellationToken);

        if (tenant == null)
        {
            logger.LogWarning("Tenant not found with ID: {Id}", request.Id);
            return Result.Failure<Tenant>("Tenant not found");
        }

        logger.LogInformation("Tenant found with ID: {Id}", request.Id);
        return Result.Success(tenant);
    }
}