namespace AgendaPlus.Domain.Entities.Bases
{
    public abstract class BaseEntityReferenceTenant : BaseEntity
    {
        public Guid TenantId { get; set; }
    }
}
