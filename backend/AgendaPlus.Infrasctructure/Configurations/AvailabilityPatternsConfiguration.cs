using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class AvailabilityPatternsConfiguration(ICurrentUserService currentUserService)
    : BaseConfiguration<AvailabilityPatterns>(currentUserService)
{
    public override void Configure(EntityTypeBuilder<AvailabilityPatterns> builder)
    {
        builder.ToTable("availability_patterns");
        base.Configure(builder);

        //Columns
        builder.Property(ap => ap.PricePerHour).HasColumnType("decimal(10,2)").HasDefaultValue(0);

        //Constraints
        builder.ToTable(ap => ap.HasCheckConstraint("chk_endTime_greater_startTime", "end_time > start_time"));
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);

        //Indexes
        builder.HasIndex(ap => ap.TenantId)
            .HasDatabaseName("idx_availability_patterns_tenant");
    }
}