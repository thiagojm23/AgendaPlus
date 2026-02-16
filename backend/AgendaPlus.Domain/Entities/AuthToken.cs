using System.Diagnostics.CodeAnalysis;
using AgendaPlus.Domain.Entities.Bases;

namespace AgendaPlus.Domain.Entities;

public class AuthToken : BaseEntityReferenceTenant
{
    private AuthToken()
    {
    }

    [SetsRequiredMembers]
    public AuthToken(User user, string? refreshToken, DateTime? expiresAt = null, int loginFailedAttempts = 0)
    {
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7);
        UserId = user.Id;
        User = user;
        LoginFailedAttempts = loginFailedAttempts;
    }

    // public required string AccessToken { get; set; }
    public string? RefreshToken { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public Guid UserId { get; init; }
    public int LoginFailedAttempts { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }
    public required User User { get; init; }

    public void UpdateRefreshToken(string? refreshToken)
    {
        RefreshToken = refreshToken;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
        LoginFailedAttempts = 0;
    }

    public void IncrementFailedAttempts()
    {
        LoginFailedAttempts++;
    }

    public void SetPasswordResetToken(string token)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;
    }

    public bool IsPasswordResetTokenValid()
    {
        return !string.IsNullOrEmpty(PasswordResetToken) &&
               PasswordResetTokenExpiresAt.HasValue &&
               PasswordResetTokenExpiresAt.Value > DateTime.UtcNow;
    }
}