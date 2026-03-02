using AgendaPlus.Application.Commands.Tenants;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Tenants;

public class UpdateTenantAddressCommandHandler(
    ILogger<UpdateTenantAddressCommandHandler> logger,
    TenantQueryBuilder tenantQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<UpdateTenantAddressCommand, Result>
{
    public async Task<Result> Handle(UpdateTenantAddressCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating address for tenant ID: {Id}", request.TenantId);

        var tenant = await tenantQueryBuilder
            .WithAdress()
            .GetByIdAsync(request.TenantId, cancellationToken);

        if (tenant == null)
        {
            logger.LogWarning("Tenant not found with ID: {Id}", request.TenantId);
            return Result.Failure("Tenant not found");
        }

        tenant.Adress.Street = request.Street;
        tenant.Adress.CityName = request.City;
        tenant.Adress.StateCode = request.State;
        tenant.Adress.Cep = request.ZipCode;
        tenant.Adress.CountryCodeAlpha2 = request.Country == "BR" ? "BR" : request.Country;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Tenant address updated successfully for ID: {Id}", request.TenantId);
        return Result.Success();
    }
}