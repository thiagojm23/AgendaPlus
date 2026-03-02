using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public static class CustomClaims
{
    public const string UserId = "userId";
    // public const string UserRole = "userRole";
}

public class User : BaseEntity
{
    //EF Core
    protected User()
    {
        Email = null!;
        FirstName = null!;
        SecondName = null!;
        PasswordHash = null!;
    }

    public User(string firstName, string lastName, string email, string passwordHash, UserRole role,
        ICollection<UserTenant> userTenants, string? phoneNumber = null, string? document = null,
        string? businessName = null)
    {
        Email = email;
        FirstName = firstName;
        SecondName = lastName;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        UserTenants = userTenants;
        PhoneNumber = phoneNumber;
        Document = document;
        BusinessName = businessName;
    }

    public Guid? TenantId { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string SecondName { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Optional fields (mainly for Owners)
    public string? PhoneNumber { get; private set; }
    public string? Document { get; private set; } // CPF/CNPJ
    public string? BusinessName { get; private set; } // Business name

    public ICollection<UserTenant> UserTenants { get; private set; } = new List<UserTenant>();
    public AuthToken? Token { get; private set; }

    public void SetAuthToken(string refreshToken)
    {
        if (Token == null) Token = new AuthToken(this, refreshToken);
        else
            Token.UpdateRefreshToken(refreshToken);
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void UpdateProfile(string firstName, string secondName, string? phoneNumber = null,
        string? document = null, string? businessName = null)
    {
        FirstName = firstName;
        SecondName = secondName;
        PhoneNumber = phoneNumber;
        Document = document;
        BusinessName = businessName;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}