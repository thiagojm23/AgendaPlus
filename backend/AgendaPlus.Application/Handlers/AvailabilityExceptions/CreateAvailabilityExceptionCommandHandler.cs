using AgendaPlus.Application.Commands.AvailabilityExceptions;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AvailabilityException = AgendaPlus.Domain.Entities.AvailabilityExceptions;

namespace AgendaPlus.Application.Handlers.AvailabilityExceptions;

public class CreateAvailabilityExceptionCommandHandler(
    ILogger<CreateAvailabilityExceptionCommandHandler> logger,
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<CreateAvailabilityExceptionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAvailabilityExceptionCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating availability exception for resource ID: {ResourceId}", request.ResourceId);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure<Guid>("User has no associated tenant");
        }

        // Verify if the resource exists and belongs to the tenant
        var resource = await context.Resources
            .FirstOrDefaultAsync(r => r.Id == request.ResourceId && r.TenantId == tenantId, cancellationToken);

        if (resource == null)
        {
            logger.LogWarning("Resource not found with ID: {ResourceId} for tenant: {TenantId}",
                request.ResourceId, tenantId);
            return Result.Failure<Guid>("Resource not found or does not belong to your tenant");
        }

        var availabilityException = new AvailabilityException(request.ResourceId)
        {
            Reason = request.Reason,
            Strategy = request.Strategy,
            StartBlockTime = request.StartBlockTime,
            EndBlockTime = request.EndBlockTime,
            OverrideStartTime = request.OverrideStartTime,
            OverrideEndTime = request.OverrideEndTime,
            Price = request.Price,
            TenantId = tenantId
        };

        context.AvailabilityExceptions.Add(availabilityException);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Availability exception created successfully with ID: {Id}", availabilityException.Id);
        return Result.Success(availabilityException.Id);
    }
}