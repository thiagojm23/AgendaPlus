using AgendaPlus.Application.Commands.Tenants;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Tenants;

public class CreateTenantCommandHandler(
    ILogger<CreateTenantCommandHandler> logger,
    IApplicationDbContext context) : IRequestHandler<CreateTenantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating tenant with name: {Name}", request.Name);

        var tenant = new Tenant
        {
            Name = request.Name,
            TimeZone = request.TimeZone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Adress = null! // Will be populated below
        };

        var address = new TenantAdress
        {
            Street = request.Street,
            Cep = request.ZipCode,
            StateCode = request.State,
            CityName = request.City,
            CountryCodeAlpha2 = request.Country,
            Tenant = tenant,
            TenantId = tenant.Id
        };

        tenant.Adress = address;

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Tenant created successfully with ID: {Id}", tenant.Id);
        return Result.Success(tenant.Id);
    }
}