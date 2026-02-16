using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class AuthTokenQueryBuilder : BaseQueryBuilder<AuthToken>
{
    public AuthTokenQueryBuilder(IApplicationDbContext context) : base(context) { }

    public AuthTokenQueryBuilder WithUser()
    {
        Query = Query.Include(t => t.User);
        return this;
    }

    public AuthTokenQueryBuilder ApplyFilter(AuthTokenFilter? filter)
    {
        if (filter == null) return this;

        if (filter.UserId.HasValue)
            Query = Query.Where(t => t.UserId == filter.UserId.Value);

        if (filter.SomenteAtivos.HasValue && filter.SomenteAtivos.Value)
            Query = Query.Where(t => t.ExpiresAt > DateTime.UtcNow);

        return this;
    }

    public async Task<AuthToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        return await Query.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken, ct);
    }

    public async Task<AuthToken?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await Query.FirstOrDefaultAsync(t => t.UserId == userId, ct);
    }
}
