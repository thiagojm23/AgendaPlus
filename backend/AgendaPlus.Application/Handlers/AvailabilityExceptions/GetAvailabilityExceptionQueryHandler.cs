using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.Queries.AvailabilityExceptions;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using AvailabilityException = AgendaPlus.Domain.Entities.AvailabilityExceptions;

namespace AgendaPlus.Application.Handlers.AvailabilityExceptions;

public class GetAvailabilityExceptionQueryHandler(
    ILogger<GetAvailabilityExceptionQueryHandler> logger,
    AvailabilityExceptionsQueryBuilder availabilityExceptionsQueryBuilder,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetAvailabilityExceptionQuery, Result<AvailabilityException>>
{
    public async Task<Result<AvailabilityException>> Handle(GetAvailabilityExceptionQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting availability exception with ID: {Id}", request.Id);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure<AvailabilityException>("User has no associated tenant");
        }

        var availabilityException = await availabilityExceptionsQueryBuilder
            .AsNoTracking()
            .GetByIdAsync(request.Id, cancellationToken);

        if (availabilityException == null || availabilityException.TenantId != tenantId)
        {
            logger.LogWarning("Availability exception not found with ID: {Id}", request.Id);
            return Result.Failure<AvailabilityException>("Availability exception not found");
        }

        logger.LogInformation("Availability exception found with ID: {Id}", request.Id);
        return Result.Success(availabilityException);
    }
}