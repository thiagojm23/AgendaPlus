using AgendaPlus.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public abstract class BaseQueryBuilder<TEntity> where TEntity : class
{
    protected readonly IApplicationDbContext Context;
    public IQueryable<TEntity> Query { get; protected set; }

    protected BaseQueryBuilder(IApplicationDbContext context)
    {
        Context = context;
        var dbContext = context as DbContext;
        Query = dbContext!.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, ct);
    }

    public async Task<List<TEntity>> ToListAsync(CancellationToken ct = default)
    {
        return await Query.ToListAsync(ct);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(CancellationToken ct = default)
    {
        return await Query.FirstOrDefaultAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await Query.CountAsync(ct);
    }
}