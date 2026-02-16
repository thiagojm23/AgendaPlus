using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Resource> Resources { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<User> Users { get; }
    DbSet<UserTenant> UserTenants { get; }
    DbSet<AvailabilityPatterns> AvailabilityPatterns { get; }
    DbSet<AvailabilityExceptions> AvailabilityExceptions { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<AuthToken> Tokens { get; }
    DbSet<TenantAdress> TenantAdresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}