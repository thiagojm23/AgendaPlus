using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Filters;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Application.QueryBuilders;

public class UserQueryBuilder : BaseQueryBuilder<User>
{
    public UserQueryBuilder(IApplicationDbContext context) : base(context) { }

    public UserQueryBuilder WithToken()
    {
        Query = Query.Include(u => u.Token);
        return this;
    }

    public UserQueryBuilder WithUserTenants()
    {
        Query = Query.Include(u => u.UserTenants);
        return this;
    }

    public UserQueryBuilder WithUserTenantsAndTenant()
    {
        Query = Query.Include(u => u.UserTenants)
                     .ThenInclude(ut => ut.Tenant);
        return this;
    }

    public UserQueryBuilder ApplyFilter(UserFilter? filter)
    {
        if (filter == null) return this;

        if (!string.IsNullOrWhiteSpace(filter.Email))
            Query = Query.Where(u => u.Email == filter.Email);

        if (!string.IsNullOrWhiteSpace(filter.FullName))
            Query = Query.Where(u => u.FullName.Contains(filter.FullName));

        if (filter.SomenteAtivos.HasValue && filter.SomenteAtivos.Value)
            Query = Query.Where(u => u.IsActive);

        if (filter.DataCadastroInicio.HasValue)
            Query = Query.Where(u => u.CreatedAt >= filter.DataCadastroInicio.Value);

        if (filter.DataCadastroFim.HasValue)
            Query = Query.Where(u => u.CreatedAt <= filter.DataCadastroFim.Value);

        return this;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await Query.FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}
