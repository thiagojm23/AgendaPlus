using AgendaPlus.Application.Commands.Users;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class CreateOwnerUserCommandHandler(
    ILogger<CreateOwnerUserCommandHandler> logger,
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<CreateOwnerUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOwnerUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating owner user with email: {Email}", request.Email);

        var existingUser = await userQueryBuilder
            .AsNoTracking()
            .GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null)
        {
            logger.LogWarning("Email already in use: {Email}", request.Email);
            return Result.Failure<Guid>("Email is already in use");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var tenant = new Tenant
        {
            Name = request.TenantName,
            TimeZone = request.TimeZone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Adress = null! // Will be populated below
        };

        var address = new TenantAdress
        {
            Street = request.Street,
            Cep = request.ZipCode, // ZipCode -> Cep
            StateCode = request.State, // State -> StateCode
            CityName = request.City, // City -> CityName
            CountryCodeAlpha2 = request.Country, // Country -> CountryCodeAlpha2
            Tenant = tenant,
            TenantId = tenant.Id
        };

        tenant.Adress = address;

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync(cancellationToken);

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword,
            UserRole.Owner,
            new List<UserTenant>(),
            request.PhoneNumber,
            request.Document,
            request.BusinessName);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        var userTenant = new UserTenant
        {
            UserId = user.Id,
            User = user,
            TenantId = tenant.Id,
            Tenant = tenant,
            CreatedAt = DateTime.UtcNow
        };

        context.UserTenants.Add(userTenant);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Owner user created with ID: {UserId} and Tenant ID: {TenantId}",
            user.Id, tenant.Id);

        return Result.Success(user.Id);
    }
}