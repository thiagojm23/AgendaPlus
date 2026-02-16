using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class TenantAdressConfiguration(ICurrentUserService currentUserService)
    : IEntityTypeConfiguration<TenantAdress>
{
    public void Configure(EntityTypeBuilder<TenantAdress> builder)
    {
        builder.ToTable("tenants_adress");

        // Base configuration (copied from BaseConfiguration but without the Many-to-One relationship)
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasQueryFilter(x => currentUserService.TenantsId.Contains(x.TenantId));

        //Columns
        builder.Property(e => e.Cep).HasColumnType("varchar(15)").IsRequired();
        builder.Property(e => e.Street).HasColumnType("varchar(200)").IsRequired();
        builder.Property(e => e.Number).HasColumnType("varchar(20)").HasDefaultValue(null);
        builder.Property(e => e.CountryCodeAlpha2).HasColumnType("char(2)").HasDefaultValue("BR");
        builder.Property(e => e.StateCode).HasColumnType("varchar(5)").IsRequired();
        builder.Property(e => e.CityName).HasColumnType("varchar(50)").IsRequired();
        builder.Property(e => e.Latitude).HasColumnType("decimal(10,8)").HasDefaultValue(null);
        builder.Property(e => e.Longitude).HasColumnType("decimal(11,8)").HasDefaultValue(null);
        builder.Property(e => e.GooglePlaceId).HasColumnType("varchar(500)").HasDefaultValue(null);

        //Constraints
        builder.HasOne<Tenant>(ta => ta.Tenant).WithOne(t => t.Adress).HasForeignKey<TenantAdress>(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        //Indexes
        builder.HasIndex(e => new { e.StateCode, e.CityName });
        builder.HasIndex(e => e.Cep).IsUnique();
        builder.HasIndex(e => e.GooglePlaceId);
        builder.HasIndex(e => new { e.Latitude, e.Longitude });
    }
}