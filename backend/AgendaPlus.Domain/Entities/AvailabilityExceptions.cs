using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities
{
    public class AvailabilityExceptions : BaseEntityReferenceTenant
    {
        public Guid ResourceId { get; private set; }
        public string? Reason { get; set; }
        public StrategyAvailabilityException Strategy { get; set; }
        public required DateTimeOffset StartBlockTime { get; set; }
        public required DateTimeOffset EndBlockTime { get; set; }

        //Overrides
        public TimeOnly? OverrideStartTime { get; set; }
        public TimeOnly? OverrideEndTime { get; set; }

        private decimal? _price;
        public decimal? Price
        {
            get => _price;
            set => _price = value is decimal v ? Math.Round(v, 2) : null;
        }
    }
}
