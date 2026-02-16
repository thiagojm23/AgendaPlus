using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
    : DbContext(options), IApplicationDbContext
{
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

        modelBuilder.ApplyConfiguration(new UserConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new ResouceConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new BookingConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new AvailabilityPatternsConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new AvailabilityExceptionsConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new TenantAdressConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new AuthTokenConfiguration(currentUserService));
        modelBuilder.ApplyConfiguration(new UserTenantConfiguration(currentUserService));

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
    }
}