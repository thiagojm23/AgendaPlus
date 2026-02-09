namespace AgendaPlus.Infrastructure.Settings;

public class RabbitMQSettings
{
    public required string HostName { get; set; }
    public int Port { get; set; } = 5672;
    public required string Username { get; set; }
    public required string Password { get; set; }
}
