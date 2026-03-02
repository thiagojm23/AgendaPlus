using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        //Query Filter moved to AppDbContext.OnModelCreating
        //Columns
        builder.HasKey(u => u.Id);
        builder.Property(u => u.TenantId).IsRequired(false);
        builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.Email).IsRequired().HasColumnType("varchar(100)");
        builder.Property(u => u.PasswordHash).IsRequired().HasColumnType("varchar(255)");
        builder.Property(u => u.FirstName).HasColumnType("varchar(50)");
        builder.Property(u => u.SecondName).HasColumnType("varchar(50)");
        builder.Property(u => u.PhoneNumber).IsRequired(false).HasColumnType("varchar(20)");
        builder.Property(u => u.Document).IsRequired(false).HasColumnType("varchar(20)");
        builder.Property(u => u.BusinessName).IsRequired(false).HasColumnType("varchar(100)");
        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(false);
        builder.Property(u => u.Role).IsRequired().HasColumnType("smallint");
        builder.Property(u => u.CreatedAt).IsRequired().HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        //Constraints
        builder.HasMany(u => u.UserTenants).WithOne(ut => ut.User).HasForeignKey(u => u.UserId);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        //Indexes
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique().HasFilter("tenant_id IS NOT NULL")
            .HasDatabaseName("idx_users_tenant_email");
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");
        builder.ToTable(t => t.HasCheckConstraint("chk_users_role", "role IN (0, 1, 2)"));
    }
}