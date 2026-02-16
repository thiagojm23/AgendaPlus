using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class AvailabilityExceptionsQueryBuilder : BaseQueryBuilder<AvailabilityExceptions>
{
    public AvailabilityExceptionsQueryBuilder(IApplicationDbContext context) : base(context) { }

    public AvailabilityExceptionsQueryBuilder ApplyFilter(AvailabilityExceptionsFilter? filter)
    {
        if (filter == null) return this;

        if (filter.ResourceId.HasValue)
            Query = Query.Where(ae => ae.ResourceId == filter.ResourceId.Value);

        if (filter.DataInicio.HasValue)
            Query = Query.Where(ae => ae.StartBlockTime >= filter.DataInicio.Value);

        if (filter.DataFim.HasValue)
            Query = Query.Where(ae => ae.EndBlockTime <= filter.DataFim.Value);

        return this;
    }
}
