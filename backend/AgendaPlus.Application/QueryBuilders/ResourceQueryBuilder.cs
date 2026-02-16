using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class ResourceQueryBuilder : BaseQueryBuilder<Resource>
{
    public ResourceQueryBuilder(IApplicationDbContext context) : base(context) { }

    public ResourceQueryBuilder WithBookings()
    {
        Query = Query.Include(r => r.Bookings);
        return this;
    }

    public ResourceQueryBuilder ApplyFilter(ResourceFilter? filter)
    {
        if (filter == null) return this;

        if (filter.TenantId.HasValue)
            Query = Query.Where(r => r.TenantId == filter.TenantId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Nome))
            Query = Query.Where(r => r.Name.Contains(filter.Nome));

        if (filter.SomenteAtivos.HasValue && filter.SomenteAtivos.Value)
            Query = Query.Where(r => r.IsActive);

        return this;
    }
}
