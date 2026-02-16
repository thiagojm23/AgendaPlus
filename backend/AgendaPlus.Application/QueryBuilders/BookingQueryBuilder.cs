using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class BookingQueryBuilder : BaseQueryBuilder<Booking>
{
    public BookingQueryBuilder(IApplicationDbContext context) : base(context) { }

    public BookingQueryBuilder WithResource()
    {
        Query = Query.Include(b => b.Resource);
        return this;
    }

    public BookingQueryBuilder ApplyFilter(BookingFilter? filter)
    {
        if (filter == null) return this;

        if (filter.TenantId.HasValue)
            Query = Query.Where(b => b.TenantId == filter.TenantId.Value);

        if (filter.ResourceId.HasValue)
            Query = Query.Where(b => b.ResourceId == filter.ResourceId.Value);

        if (filter.Status.HasValue)
            Query = Query.Where(b => b.Status == filter.Status.Value);

        if (filter.DataInicio.HasValue)
            Query = Query.Where(b => b.StartBookingDateTime >= filter.DataInicio.Value);

        if (filter.DataFim.HasValue)
            Query = Query.Where(b => b.EndBookingDateTime <= filter.DataFim.Value);

        return this;
    }
}
