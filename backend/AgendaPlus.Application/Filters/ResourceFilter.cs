namespace AgendaPlus.Application.Filters;

public class ResourceFilter
{
    public Guid? TenantId { get; set; }
    public string? Nome { get; set; }
    public bool? SomenteAtivos { get; set; }
}
