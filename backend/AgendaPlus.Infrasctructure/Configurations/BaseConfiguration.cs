using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class BaseConfiguration<T>(ICurrentUserService currentUserService)
    : IEntityTypeConfiguration<T> where T : BaseEntityReferenceTenant
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => currentUserService.TenantsId.Contains(x.TenantId));
    }
}