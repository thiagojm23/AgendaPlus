namespace AgendaPlus.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    IEnumerable<Guid> TenantsId { get; }
    bool IsAuthenticated { get; }
}