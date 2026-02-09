using System.Text.Json;
using AgendaPlus.Application.Contracts;
using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Domain.Enums;
using MassTransit;

namespace AgendaPlus.WebApi.Workers;

public class OutboxProcessorWorker(
    IServiceProvider serviceProvider,
    ILogger<OutboxProcessorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("OutboxProcessorWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var pendingMessages = await outboxRepository.GetPendingMessagesAsync(batchSize: 10);

                foreach (var message in pendingMessages)
                {
                    try
                    {
                        logger.LogInformation("Processing outbox message: {MessageId}, Type: {Type}", 
                            message.Id, message.Type);

                        await outboxRepository.UpdateStatusAsync(message.Id, OutboxStatus.Processing);

                        // Publicar baseado no tipo
                        await PublishMessageAsync(publishEndpoint, message);

                        await outboxRepository.UpdateStatusAsync(message.Id, OutboxStatus.Processed);

                        logger.LogInformation("Outbox message processed successfully: {MessageId}", message.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing outbox message: {MessageId}", message.Id);

                        var status = message.RetryCount >= 3 ? OutboxStatus.Dead : OutboxStatus.Retrying;
                        await outboxRepository.UpdateStatusAsync(message.Id, status, ex.Message);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OutboxProcessorWorker loop");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        logger.LogInformation("OutboxProcessorWorker stopped");
    }

    private async Task PublishMessageAsync(IPublishEndpoint publishEndpoint, Domain.Entities.OutboxMessage message)
    {
        switch (message.Type)
        {
            case OutboxMessageType.ForgotPasswordEmail:
                var emailContent = JsonSerializer.Deserialize<ForgotPasswordEmailContent>(
                    message.Content.RootElement.GetRawText());
                
                if (emailContent != null)
                {
                    var emailMessage = new ForgotPasswordEmailMessage
                    {
                        Email = emailContent.Email,
                        UserName = emailContent.UserName,
                        ResetToken = emailContent.ResetToken
                    };

                    await publishEndpoint.Publish(emailMessage);
                    logger.LogInformation("Published ForgotPasswordEmail message for: {Email}", emailContent.Email);
                }
                break;

            default:
                logger.LogWarning("Unknown message type: {Type}", message.Type);
                break;
        }
    }

    private class ForgotPasswordEmailContent
    {
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string ResetToken { get; set; }
    }
}
