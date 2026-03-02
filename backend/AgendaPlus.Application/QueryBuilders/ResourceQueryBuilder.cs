using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class ResourceQueryBuilder(IApplicationDbContext context) : BaseQueryBuilder<Resource>(context)
{
    public ResourceQueryBuilder AsNoTracking()
    {
        Query = Query.AsNoTracking();
        return this;
    }

    public ResourceQueryBuilder AsSplitQuery()
    {
        Query = Query.AsSplitQuery();
        return this;
    }

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

    public ResourceQueryBuilder WithIsActive(bool? isActive)
    {
        if (isActive.HasValue)
            Query = Query.Where(r => r.IsActive == isActive.Value);
        return this;
    }

    public ResourceQueryBuilder WithResourceType(ResourceType? resourceType)
    {
        if (resourceType.HasValue)
            Query = Query.Where(r => r.ResourceType == resourceType.Value);
        return this;
    }
}