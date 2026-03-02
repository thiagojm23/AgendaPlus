using AgendaPlus.Application.Commands.Tenants;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Tenants;

public class DeleteTenantCommandHandler(
    ILogger<DeleteTenantCommandHandler> logger,
    TenantQueryBuilder tenantQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<DeleteTenantCommand, Result>
{
    public async Task<Result> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting tenant with ID: {Id}", request.Id);

        var tenant = await tenantQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (tenant == null)
        {
            logger.LogWarning("Tenant not found with ID: {Id}", request.Id);
            return Result.Failure("Tenant not found");
        }

        context.Tenants.Remove(tenant);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Tenant deleted successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}