using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Application.Interfaces.Repositories;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage message);
    Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 10);
    Task UpdateAsync(OutboxMessage message);
    Task UpdateStatusAsync(Guid id, OutboxStatus status, string? error = null);
}
