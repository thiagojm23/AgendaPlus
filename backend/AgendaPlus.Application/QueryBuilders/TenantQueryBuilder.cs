using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class TenantQueryBuilder : BaseQueryBuilder<Tenant>
{
    public TenantQueryBuilder(IApplicationDbContext context) : base(context) { }

    public TenantQueryBuilder WithAdress()
    {
        Query = Query.Include(t => t.Adress);
        return this;
    }

    public TenantQueryBuilder WithUserTenants()
    {
        Query = Query.Include(t => t.UserTenants);
        return this;
    }

    public TenantQueryBuilder WithUserTenantsAndUsers()
    {
        Query = Query.Include(t => t.UserTenants)
                     .ThenInclude(ut => ut.User);
        return this;
    }

    public TenantQueryBuilder ApplyFilter(TenantFilter? filter)
    {
        if (filter == null) return this;

        if (!string.IsNullOrWhiteSpace(filter.Nome))
            Query = Query.Where(t => t.Name.Contains(filter.Nome));

        if (filter.SomenteAtivos.HasValue && filter.SomenteAtivos.Value)
            Query = Query.Where(t => t.IsActive);

        return this;
    }
}
