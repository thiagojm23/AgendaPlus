using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public class Resource : BaseEntityReferenceTenant
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required ResourceType ResourceType { get; set; }
    public required DaysOfWeekBitwise OpenDays { get; set; }
    public bool IsActive { get; set; } = false;
    public ICollection<Booking> Bookings { get; set; } = [];
}