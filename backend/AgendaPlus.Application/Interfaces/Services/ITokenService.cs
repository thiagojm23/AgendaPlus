using AgendaPlus.Domain.Entities;

namespace AgendaPlus.Application.Interfaces.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync(User user);
    Guid GetUserIdFromToken(string token);
}