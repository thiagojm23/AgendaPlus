using System.Linq.Expressions;
using AgendaPlus.Domain.Entities;

namespace AgendaPlus.Application.Interfaces.Repositories;

public interface ITokenRepository : IBaseRepository<AuthToken>
{
    Task<AuthToken?> GetByRefreshTokenAsync(string refreshToken,
        params Expression<Func<AuthToken, object?>>[] includes);

    Task<AuthToken?> GetByUserIdAsync(Guid userId, params Expression<Func<AuthToken, object>>[] includes);
}