using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using AgendaPlus.Domain.Entities.Bases;
using AgendaPlus.Domain.Enums;

namespace AgendaPlus.Domain.Entities
{
    public class OutboxMessage : BaseEntity
    {
        public OutboxMessageType Type { get; set; }

        [Column(TypeName = "jsonb")] //Definir posteriormente um Objeto concreto
        public required JsonDocument Content { get; set; } // JSON String ou JsonDocument
        public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
        public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ProcessedOn { get; set; }
        public string? Error { get; set; }
        public int RetryCount { get; set; } = 0;
    }
}
