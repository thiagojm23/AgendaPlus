using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Application.Filters;

public class AuthTokenFilter
{
    public Guid? UserId { get; set; }
    public bool? SomenteAtivos { get; set; }
}
