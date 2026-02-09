using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public static class CustomClaims
{
    public const string UserId = "userId";
    // public const string UserRole = "userRole";
}

public class User : BaseEntityReferenceTenant
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public required UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserTenant> UserTenants { get; set; } = [];
    public AuthToken? Token { get; set; }

    public void SetAuthToken(string refreshToken)
    {
        if (Token == null) Token = new AuthToken(this, refreshToken);
        else
            Token.UpdateRefreshToken(refreshToken);
    }
}