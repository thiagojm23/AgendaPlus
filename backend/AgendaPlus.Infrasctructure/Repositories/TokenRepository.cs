using System.Data;
using System.Linq.Expressions;
using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Infrastructure.Repositories;

public class TokenRepository(AppDbContext context, IDbConnection dbConnection) : ITokenRepository
{
    public async Task<AuthToken?> GetByIdAsync(Guid id, params Expression<Func<AuthToken, object>>[] includes)
    {
        var query = context.Set<AuthToken>().AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<AuthToken>> GetAllAsync(params Expression<Func<AuthToken, object>>[] includes)
    {
        var query = context.Set<AuthToken>().AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<AuthToken?> GetByRefreshTokenAsync(string refreshToken,
        params Expression<Func<AuthToken, object?>>[] includes)
    {
        var query = context.Set<AuthToken>().AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
    }

    public async Task<AuthToken?> GetByUserIdAsync(Guid userId, params Expression<Func<AuthToken, object>>[] includes)
    {
        var query = context.Set<AuthToken>().AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task AddAsync(AuthToken entity)
    {
        await context.Set<AuthToken>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AuthToken entity)
    {
        context.Set<AuthToken>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AuthToken entity)
    {
        context.Set<AuthToken>().Remove(entity);
        await context.SaveChangesAsync();
    }
}