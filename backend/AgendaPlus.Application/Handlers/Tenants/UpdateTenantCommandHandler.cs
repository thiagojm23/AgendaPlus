using AgendaPlus.Application.Commands.Tenants;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Tenants;

public class UpdateTenantCommandHandler(
    ILogger<UpdateTenantCommandHandler> logger,
    TenantQueryBuilder tenantQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<UpdateTenantCommand, Result>
{
    public async Task<Result> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating tenant with ID: {Id}", request.Id);

        var tenant = await tenantQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (tenant == null)
        {
            logger.LogWarning("Tenant not found with ID: {Id}", request.Id);
            return Result.Failure("Tenant not found");
        }

        tenant.Name = request.Name;
        tenant.TimeZone = request.TimeZone;
        tenant.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Tenant updated successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}