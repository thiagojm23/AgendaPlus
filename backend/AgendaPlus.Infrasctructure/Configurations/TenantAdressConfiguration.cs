using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class TenantAdressConfiguration(ICurrentUserService currentUserService)
    : BaseConfiguration<TenantAdress>(currentUserService)
{
    public override void Configure(EntityTypeBuilder<TenantAdress> builder)
    {
        builder.ToTable("tenants_adress");

        base.Configure(builder);

        //Columns
        builder.Property(e => e.Cep).HasColumnType("varchar(15").IsRequired();
        builder.Property(e => e.Street).HasColumnType("varchar(200)").IsRequired();
        builder.Property(e => e.Number).HasColumnType("varchar(20)").HasDefaultValue(null);
        builder.Property(e => e.CountryCodeAlpha2).HasColumnType("char(2)").HasDefaultValue("BR");
        builder.Property(e => e.StateCode).HasColumnType("varchar(5)").IsRequired();
        builder.Property(e => e.CityName).HasColumnType("varchar(50)").IsRequired();
        builder.Property(e => e.Latitude).HasColumnType("decimal(10,8)").HasDefaultValue(null);
        builder.Property(e => e.Longitude).HasColumnType("decimal(11,8)").HasDefaultValue(null);
        builder.Property(e => e.GooglePlaceId).HasColumnType("varchar(500)").HasDefaultValue(null);

        //Constraints
        builder.HasOne<Tenant>().WithOne().HasForeignKey<TenantAdress>(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        //Indexes
        builder.HasIndex(e => new { e.StateCode, e.CityName });
        builder.HasIndex(e => e.Cep).IsUnique();
        builder.HasIndex(e => e.GooglePlaceId);
        builder.HasIndex(e => new { e.Latitude, e.Longitude });
        builder.HasIndex(e => e.TenantId);
    }
}