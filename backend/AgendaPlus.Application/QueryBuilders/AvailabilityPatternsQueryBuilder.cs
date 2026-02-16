using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class AvailabilityPatternsQueryBuilder : BaseQueryBuilder<AvailabilityPatterns>
{
    public AvailabilityPatternsQueryBuilder(IApplicationDbContext context) : base(context) { }

    public AvailabilityPatternsQueryBuilder ApplyFilter(AvailabilityPatternsFilter? filter)
    {
        if (filter == null) return this;

        if (filter.ResourceId.HasValue)
            Query = Query.Where(ap => ap.ResourceId == filter.ResourceId.Value);

        return this;
    }
}
