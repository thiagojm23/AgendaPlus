using AgendaPlus.Domain.Entities.Bases;

namespace AgendaPlus.Domain.Entities;

public class UserTenant : BaseEntityReferenceTenant
{
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public required Tenant Tenant { get; set; }
    public DateTime CreatedAt { get; set; }
}