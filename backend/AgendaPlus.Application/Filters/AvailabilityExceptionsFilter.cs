namespace AgendaPlus.Application.Filters;

public class AvailabilityExceptionsFilter
{
    public Guid? ResourceId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
