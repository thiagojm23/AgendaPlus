using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class ResouceConfiguration(ICurrentUserService currentUserService)
    : BaseConfiguration<Resource>(currentUserService)
{
    public override void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("resources");

        base.Configure(builder);

        //Columns
        builder.Property(r => r.Name).IsRequired().HasColumnType("varchar(50)");
        builder.Property(r => r.Description).HasColumnType("varchar(250)");
        builder.Property(r => r.IsActive).HasDefaultValue(false).IsRequired();
        builder.Property(r => r.Description).HasColumnType("varchar(500)");

        //Constraints
        builder.ToTable(r => r.HasCheckConstraint("chk_openDays_positive", "open_days >= 0"));
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);

        //Indexes
        builder.HasIndex(r => r.TenantId)
            .HasDatabaseName("idx_resources_tenant");
    }
}