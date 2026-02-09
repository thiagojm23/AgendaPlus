using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class BookingConfiguration(ICurrentUserService currentUserService)
    : BaseConfiguration<Booking>(currentUserService)
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        base.Configure(builder);

        //Columns
        builder.Property(b => b.StartBookingDateTime).HasColumnType("timestamp without time zone").IsRequired();
        builder.Property(b => b.EndBookingDateTime).HasColumnType("timestamp without time zone").IsRequired();
        builder.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)").IsRequired();
        builder.OwnsOne(b => b.CustomerData, cb => { cb.ToJson(); });
        builder.Property<DateTimeOffset>("created_at").HasColumnType("timestamptz").HasDefaultValue("CURRENT_TIMESTAMP")
            .IsRequired();

        //Constraints
        builder.HasOne(b => b.Resource)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.ResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.ToTable(b => b.HasCheckConstraint("chk_booking_time_logic", "end_time > start_time"));

        //Indexes
        builder.HasIndex(b => b.TenantId)
            .HasDatabaseName("idx_bookings_tenant");
    }
}