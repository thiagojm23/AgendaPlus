using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.Infrastructure.Repositories;

public class OutboxMessageRepository(AppDbContext context) : IOutboxMessageRepository
{
    public async Task AddAsync(OutboxMessage message)
    {
        await context.OutboxMessages.AddAsync(message);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 10)
    {
        return await context.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Pending || m.Status == OutboxStatus.Retrying)
            .OrderBy(m => m.OccurredOn)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task UpdateAsync(OutboxMessage message)
    {
        context.OutboxMessages.Update(message);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid id, OutboxStatus status, string? error = null)
    {
        var message = await context.OutboxMessages.FindAsync(id);
        if (message != null)
        {
            message.Status = status;
            message.Error = error;
            
            if (status == OutboxStatus.Processed)
                message.ProcessedOn = DateTimeOffset.UtcNow;
            
            if (status == OutboxStatus.Failed || status == OutboxStatus.Retrying)
                message.RetryCount++;

            await context.SaveChangesAsync();
        }
    }
}
