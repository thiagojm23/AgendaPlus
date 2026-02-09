using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            //Columns
            builder.Property(x => x.Content).HasColumnType("jsonb");
            builder.Property(x => x.OccurredOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
