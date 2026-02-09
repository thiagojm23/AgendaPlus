using AgendaPlus.Domain.Entities.Bases;

namespace AgendaPlus.Domain.Entities;

public class AuthToken : BaseEntity
{
    public AuthToken(User user, string? refreshToken, DateTime? expiresAt = null, int failedAttempts = 0)
    {
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7);
        UserId = user.Id;
        User = user;
        FailedAttempts = failedAttempts;
    }

    // public required string AccessToken { get; set; }
    public string? RefreshToken { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public Guid UserId { get; init; }
    public int FailedAttempts { get; private set; }
    public User User { get; init; }

    public void UpdateRefreshToken(string? refreshToken)
    {
        RefreshToken = refreshToken;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
        FailedAttempts = 0;
    }

    public void IncrementFailedAttempts()
    {
        FailedAttempts++;
    }
}