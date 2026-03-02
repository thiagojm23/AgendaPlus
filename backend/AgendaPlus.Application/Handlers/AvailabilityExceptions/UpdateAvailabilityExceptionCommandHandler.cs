using AgendaPlus.Application.Commands.AvailabilityExceptions;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.AvailabilityExceptions;

public class UpdateAvailabilityExceptionCommandHandler(
    ILogger<UpdateAvailabilityExceptionCommandHandler> logger,
    AvailabilityExceptionsQueryBuilder availabilityExceptionsQueryBuilder,
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateAvailabilityExceptionCommand, Result>
{
    public async Task<Result> Handle(UpdateAvailabilityExceptionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating availability exception with ID: {Id}", request.Id);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure("User has no associated tenant");
        }

        var availabilityException = await availabilityExceptionsQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (availabilityException == null || availabilityException.TenantId != tenantId)
        {
            logger.LogWarning("Availability exception not found with ID: {Id}", request.Id);
            return Result.Failure("Availability exception not found");
        }

        availabilityException.Reason = request.Reason;
        availabilityException.Strategy = request.Strategy;
        availabilityException.StartBlockTime = request.StartBlockTime;
        availabilityException.EndBlockTime = request.EndBlockTime;
        availabilityException.OverrideStartTime = request.OverrideStartTime;
        availabilityException.OverrideEndTime = request.OverrideEndTime;
        availabilityException.Price = request.Price;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Availability exception updated successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}