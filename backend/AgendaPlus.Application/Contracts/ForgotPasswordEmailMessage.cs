namespace AgendaPlus.Application.Contracts;

public record ForgotPasswordEmailMessage
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string ResetToken { get; init; }
}
