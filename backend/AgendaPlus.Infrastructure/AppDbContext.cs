using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
    : DbContext(options), IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTenant> UserTenants { get; set; }
    public DbSet<AvailabilityPatterns> AvailabilityPatterns { get; set; }
    public DbSet<AvailabilityExceptions> AvailabilityExceptions { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<AuthToken> Tokens { get; set; }
    public DbSet<TenantAdress> TenantAdresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ResouceConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new AvailabilityPatternsConfiguration());
        modelBuilder.ApplyConfiguration(new AvailabilityExceptionsConfiguration());
        modelBuilder.ApplyConfiguration(new TenantAdressConfiguration());
        modelBuilder.ApplyConfiguration(new AuthTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserTenantConfiguration());

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new TenantConfiguration());

        // Centralized query filters using DbContext field (scoped per-request)
        // EF Core treats DbContext field accesses as "context accessors" and re-evaluates per query
        // User has no tenant filter since it's queried globally (unique email, login, etc.)

        modelBuilder.Entity<UserTenant>()
            .HasQueryFilter(x => !_currentUserService.IsAuthenticated ||
                                 _currentUserService.TenantsId.Contains(x.TenantId));

        modelBuilder.Entity<TenantAdress>()
            .HasQueryFilter(x => !_currentUserService.IsAuthenticated ||
                                 _currentUserService.TenantsId.Contains(x.TenantId));

        modelBuilder.Entity<Resource>()
            .HasQueryFilter(x => !_currentUserService.IsAuthenticated ||
                                 _currentUserService.TenantsId.Contains(x.TenantId));

        modelBuilder.Entity<Booking>()
            .HasQueryFilter(x => !_currentUserService.IsAuthenticated ||
                                 _currentUserService.TenantsId.Contains(x.TenantId));

        modelBuilder.Entity<AvailabilityPatterns>()
            .HasQueryFilter(x => !_currentUserService.IsAuthenticated ||
                                 _currentUserService.TenantsId.Contains(x.TenantId));

        modelBuilder.Entity<AvailabilityExceptions>()
            .HasQueryFilter(x => !_currentUserService.IsAuthenticated ||
                                 _currentUserService.TenantsId.Contains(x.TenantId));
    }
}