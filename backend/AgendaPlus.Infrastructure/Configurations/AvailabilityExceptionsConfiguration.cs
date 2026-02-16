using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class AvailabilityExceptionsConfiguration(ICurrentUserService currentUserService)
    : BaseConfiguration<AvailabilityExceptions>(currentUserService)
{
    public override void Configure(EntityTypeBuilder<AvailabilityExceptions> builder)
    {
        builder.ToTable("availability_exceptions");

        base.Configure(builder);

        //Columns
        builder.Property(ae => ae.Reason).HasColumnType("varchar(500)").HasDefaultValue(null);
        builder.Property(ae => ae.Strategy).HasColumnType("smallint");
        builder.Property(ae => ae.StartBlockTime);
        builder.Property(ae => ae.EndBlockTime);
        builder.Property(ae => ae.OverrideStartTime).HasColumnType("time").HasDefaultValue(null)
            .HasColumnName("new_start_time");
        builder.Property(ae => ae.OverrideEndTime).HasColumnType("time").HasDefaultValue(null)
            .HasColumnName("new_end_time");
        builder.Property(ae => ae.Price).HasColumnType("decimal(10,2)").HasDefaultValue(null)
            .HasColumnName("new_price_per_hour");

        //Constraints
        builder.ToTable(ae => ae.HasCheckConstraint("chk_strategy_logic",
            "(strategy = 0) OR " +
            "(strategy = 1 AND new_start_time IS NOT NULL AND new_end_time IS NOT NULL) OR " +
            "(strategy = 2 AND new_price_per_hour IS NOT NULL) OR " +
            "(strategy = 3 AND new_start_time IS NOT NULL AND new_end_time IS NOT NULL AND new_price_per_hour IS NOT NULL)"));
    }
}