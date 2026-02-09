using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("tenants");

            //Columns
            builder.Property(t => t.Name).IsRequired().HasColumnType("nvarchar(100)");
            builder.Property(t => t.Settings).HasDefaultValueSql("'{}'::jsonb").HasColumnType("jsonb");
            builder.Property(t => t.TimeZone).IsRequired().HasColumnType("nvarchar(50)");
            builder.Property(t => t.IsActive).IsRequired().HasDefaultValue(false);
            builder.Property(t => t.CreatedAt).IsRequired().HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            //Constraints
            builder.HasOne(t => t.Adresses)
                .WithOne()
                .HasForeignKey<TenantAdress>(a => a.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.UserTenants)
                .WithOne(ut => ut.Tenant)
                .HasForeignKey(ut => ut.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
