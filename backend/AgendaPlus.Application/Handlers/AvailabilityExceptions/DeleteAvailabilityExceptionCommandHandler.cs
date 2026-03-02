using AgendaPlus.Application.Commands.AvailabilityExceptions;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.AvailabilityExceptions;

public class DeleteAvailabilityExceptionCommandHandler(
    ILogger<DeleteAvailabilityExceptionCommandHandler> logger,
    AvailabilityExceptionsQueryBuilder availabilityExceptionsQueryBuilder,
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteAvailabilityExceptionCommand, Result>
{
    public async Task<Result> Handle(DeleteAvailabilityExceptionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting availability exception with ID: {Id}", request.Id);

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

        context.AvailabilityExceptions.Remove(availabilityException);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Availability exception deleted successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}