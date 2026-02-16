using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class OutboxMessageQueryBuilder : BaseQueryBuilder<OutboxMessage>
{
    public OutboxMessageQueryBuilder(IApplicationDbContext context) : base(context) { }

    public OutboxMessageQueryBuilder ApplyFilter(OutboxMessageFilter? filter)
    {
        if (filter == null) return this;

        if (filter.Status.HasValue)
            Query = Query.Where(m => m.Status == filter.Status.Value);

        if (filter.Type.HasValue)
            Query = Query.Where(m => m.Type == filter.Type.Value);

        if (filter.DataInicio.HasValue)
            Query = Query.Where(m => m.OccurredOn >= filter.DataInicio.Value);

        if (filter.DataFim.HasValue)
            Query = Query.Where(m => m.OccurredOn <= filter.DataFim.Value);

        return this;
    }

    public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 10, CancellationToken ct = default)
    {
        return await Query
            .Where(m => m.Status == OutboxStatus.Pending || m.Status == OutboxStatus.Retrying)
            .OrderBy(m => m.OccurredOn)
            .Take(batchSize)
            .ToListAsync(ct);
    }
}
