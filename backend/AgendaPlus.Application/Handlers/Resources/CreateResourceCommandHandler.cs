using AgendaPlus.Application.Commands.Resources;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Resources;

public class CreateResourceCommandHandler(
    ILogger<CreateResourceCommandHandler> logger,
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<CreateResourceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating resource with name: {Name}", request.Name);

        var tenantId = currentUserService.TenantsId.FirstOrDefault();
        if (tenantId == Guid.Empty)
        {
            logger.LogWarning("User has no associated tenant");
            return Result.Failure<Guid>("User has no associated tenant");
        }

        var resource = new Resource
        {
            Name = request.Name,
            Description = request.Description,
            ResourceType = request.ResourceType,
            OpenDays = request.OpenDays,
            IsActive = request.IsActive,
            TenantId = tenantId
        };

        context.Resources.Add(resource);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Resource created successfully with ID: {Id}", resource.Id);
        return Result.Success(resource.Id);
    }
}