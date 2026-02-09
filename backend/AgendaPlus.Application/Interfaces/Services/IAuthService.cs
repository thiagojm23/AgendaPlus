using AgendaPlus.Domain.Common;

namespace AgendaPlus.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result> ValidateRefreshTokenAsync(string refreshToken);
}