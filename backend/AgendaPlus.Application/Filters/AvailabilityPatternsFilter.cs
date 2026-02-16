namespace AgendaPlus.Application.Filters;

public class AvailabilityPatternsFilter
{
    public Guid? ResourceId { get; set; }
    public DayOfWeek? DiaDaSemana { get; set; }
    public bool? SomenteAtivos { get; set; }
}
