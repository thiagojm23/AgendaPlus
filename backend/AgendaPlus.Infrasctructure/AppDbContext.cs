using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}