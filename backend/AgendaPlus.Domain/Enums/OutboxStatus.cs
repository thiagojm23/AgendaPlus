namespace AgendaPlus.Domain.Enums
{
    public enum OutboxStatus
    {
        Pending = 1,
        Processing,
        Processed,
        Failed,
        Retrying,
        Dead
    }
}
