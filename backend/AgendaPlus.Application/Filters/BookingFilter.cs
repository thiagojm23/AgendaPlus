using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Application.Filters;

public class BookingFilter
{
    public Guid? TenantId { get; set; }
    public Guid? ResourceId { get; set; }
    public Guid? UserId { get; set; }
    public BookingStatus? Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
