using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Application.Filters;

public class OutboxMessageFilter
{
    public OutboxStatus? Status { get; set; }
    public OutboxMessageType? Type { get; set; }
    public DateTimeOffset? DataInicio { get; set; }
    public DateTimeOffset? DataFim { get; set; }
}
