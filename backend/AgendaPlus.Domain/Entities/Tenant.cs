using System.ComponentModel.DataAnnotations.Schema;
using AgendaPlus.Domain.Entities.Bases;

namespace AgendaPlus.Domain.Entities;

public class Tenant : BaseEntity
{
    public required string Name { get; set; }

    [Column(TypeName = "jsonb")] public TenantSettings Settings { get; set; } = new();

    public required string TimeZone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required TenantAdress Adress { get; set; }
    public ICollection<UserTenant> UserTenants { get; set; } = [];
}