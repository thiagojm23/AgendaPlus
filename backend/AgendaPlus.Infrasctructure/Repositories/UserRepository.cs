using System.Data;
using System.Linq.Expressions;
using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Infrastructure.Repositories;

public class UserRepository(AppDbContext context, IDbConnection dbConnection) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, params Expression<Func<User, object>>[] includes)
    {
        var query = context.Users.AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync(params Expression<Func<User, object>>[] includes)
    {
        var query = context.Users.AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email, params Expression<Func<User, object>>[] includes)
    {
        var query = context.Users.AsQueryable();
        foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User entity)
    {
        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        context.Users.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        context.Users.Remove(entity);
        await context.SaveChangesAsync();
    }
}