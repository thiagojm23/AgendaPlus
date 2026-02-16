using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class UserConfiguration(ICurrentUserService currentUserService) : BaseConfiguration<User>(currentUserService)
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        base.Configure(builder);

        //Columns
        builder.Property(u => u.Email).IsRequired().HasColumnType("varchar(100)");
        builder.Property(u => u.PasswordHash).IsRequired().HasColumnType("varchar(255)");
        builder.Property(u => u.FullName).HasColumnType("varchar(100)");
        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(false);
        builder.Property(u => u.Role).IsRequired().HasColumnType("smallint");
        builder.Property(u => u.CreatedAt).IsRequired().HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        //Constraints
        builder.HasMany(u => u.UserTenants).WithOne(ut => ut.User).HasForeignKey(u => u.UserId);

        //Indexes
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique().HasDatabaseName("idx_users_tenant_email");
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");
        builder.ToTable(t => t.HasCheckConstraint("chk_users_role", "role IN (0, 1, 2)"));
    }
}