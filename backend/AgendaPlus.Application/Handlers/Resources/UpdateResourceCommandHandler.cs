using AgendaPlus.Application.Commands.Resources;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Resources;

public class UpdateResourceCommandHandler(
    ILogger<UpdateResourceCommandHandler> logger,
    ResourceQueryBuilder resourceQueryBuilder,
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateResourceCommand, Result>
{
    public async Task<Result> Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating resource with ID: {Id}", request.Id);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure("User has no associated tenant");
        }

        var resource = await resourceQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (resource == null || resource.TenantId != tenantId)
        {
            logger.LogWarning("Resource not found with ID: {Id}", request.Id);
            return Result.Failure("Resource not found");
        }

        resource.Name = request.Name;
        resource.Description = request.Description;
        resource.ResourceType = request.ResourceType;
        resource.OpenDays = request.OpenDays;
        resource.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Resource updated successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}