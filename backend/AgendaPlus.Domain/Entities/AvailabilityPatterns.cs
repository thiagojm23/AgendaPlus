using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public class AvailabilityPatterns : BaseEntityReferenceTenant
{
    private decimal _price_per_hour;
    public Guid ResourceId { get; private set; }
    public required DaysOfWeekBitwise DayOfWeek { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
    public required Resource Resource { get; set; }

    public decimal PricePerHour
    {
        get => _price_per_hour;
        set => _price_per_hour = Math.Round(value, 2);
    }
}