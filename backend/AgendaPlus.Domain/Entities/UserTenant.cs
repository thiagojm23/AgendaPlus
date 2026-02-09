namespace AgendaPlus.Domain.Entities;

public class UserTenant
{
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public Guid TenantId { get; set; }
    public required Tenant Tenant { get; set; }
    public DateTime CreatedAt { get; set; }
}