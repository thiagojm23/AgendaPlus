using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class BookingConfiguration : BaseConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        base.Configure(builder);

        //Columns
        builder.Property(b => b.UserId).IsRequired(false);
        builder.Property(b => b.ReservationCode).IsRequired().HasColumnType("varchar(20)");
        builder.Property(b => b.StartBookingDateTime).IsRequired();
        builder.Property(b => b.EndBookingDateTime).IsRequired();
        builder.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(b => b.Status).IsRequired().HasColumnType("smallint");
        builder.OwnsOne(b => b.CustomerData, cb => { cb.ToJson(); });
        builder.Property<DateTimeOffset>("created_at").HasColumnType("timestamptz")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        //Constraints
        builder.HasOne(b => b.Resource)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.ResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.ToTable(b =>
            b.HasCheckConstraint("chk_booking_time_logic", "end_booking_date_time > start_booking_date_time"));

        //Indexes
        builder.HasIndex(b => b.TenantId)
            .HasDatabaseName("idx_bookings_tenant");
        builder.HasIndex(b => b.ReservationCode)
            .IsUnique()
            .HasDatabaseName("idx_bookings_reservation_code");
        builder.HasIndex(b => new { b.ResourceId, b.StartBookingDateTime, b.EndBookingDateTime })
            .HasDatabaseName("idx_bookings_resource_datetime");
    }
}