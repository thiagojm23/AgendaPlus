using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AgendaPlus.Application.Commands;
using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IOutboxMessageRepository outboxMessageRepository,
    ILogger<ForgotPasswordCommandHandler> logger) : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing forgot password request for email: {Email}", request.Email);

        var user = await userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            logger.LogWarning("User not found for email: {Email}", request.Email);
            // Retornar true mesmo quando usuário não existe (segurança - não revelar se email existe)
            return true;
        }

        var resetToken = GenerateResetToken();
        
        logger.LogInformation("Generated reset token for user: {UserId}", user.Id);

        var messageContent = new
        {
            Email = user.Email,
            UserName = user.FullName,
            ResetToken = resetToken
        };

        var outboxMessage = new OutboxMessage
        {
            Type = OutboxMessageType.ForgotPasswordEmail,
            Content = JsonDocument.Parse(JsonSerializer.Serialize(messageContent)),
            Status = OutboxStatus.Pending,
            OccurredOn = DateTimeOffset.UtcNow
        };

        await outboxMessageRepository.AddAsync(outboxMessage);
        
        logger.LogInformation("Outbox message created for forgot password email. User: {UserId}", user.Id);

        return true;
    }

    private static string GenerateResetToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}

