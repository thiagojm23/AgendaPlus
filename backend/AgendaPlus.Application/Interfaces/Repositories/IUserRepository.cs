using System.Linq.Expressions;
using AgendaPlus.Domain.Entities;

namespace AgendaPlus.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, params Expression<Func<User, object>>[] includes);
}