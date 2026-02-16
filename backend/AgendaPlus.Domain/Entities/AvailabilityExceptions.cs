using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities;

public class AvailabilityExceptions : BaseEntityReferenceTenant
{
    private decimal? _price;
    public Guid ResourceId { get; private set; }
    public string? Reason { get; set; }
    public StrategyAvailabilityException Strategy { get; set; }
    public required DateTime StartBlockTime { get; set; }
    public required DateTime EndBlockTime { get; set; }

    //Overrides
    public TimeOnly? OverrideStartTime { get; set; }
    public TimeOnly? OverrideEndTime { get; set; }

    public decimal? Price
    {
        get => _price;
        set => _price = value is decimal v ? Math.Round(v, 2) : null;
    }
}