using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class UserTenantConfiguration(ICurrentUserService currentUserService) : IEntityTypeConfiguration<UserTenant>
{
    public void Configure(EntityTypeBuilder<UserTenant> builder)
    {
        builder.ToTable("user_tenants");

        //Columns
        builder.HasKey(ut => new { ut.UserId, ut.TenantId });
        builder.Property(ut => ut.CreatedAt).IsRequired().HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("now()");

        //Constraints
        builder.HasOne(ut => ut.User)
            .WithMany(u => u.UserTenants)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ut => ut.Tenant)
            .WithMany(t => t.UserTenants)
            .HasForeignKey(ut => ut.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        //Query Filter
        builder.HasQueryFilter(x => currentUserService.TenantsId.Contains(x.TenantId));

        //Indexes
        builder.HasIndex(ut => ut.TenantId);
    }
}