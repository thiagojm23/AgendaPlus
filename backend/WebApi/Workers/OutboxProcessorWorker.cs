using System.Text.Json;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Contracts;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Entities;
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
            try
            {
                using var scope = serviceProvider.CreateScope();
                var outboxQueryBuilder = scope.ServiceProvider.GetRequiredService<OutboxMessageQueryBuilder>();
                var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var pendingMessages = await outboxQueryBuilder.GetPendingMessagesAsync(10, stoppingToken);

                foreach (var message in pendingMessages)
                    try
                    {
                        logger.LogInformation("Processing outbox message: {MessageId}, Type: {Type}",
                            message.Id, message.Type);

                        message.Status = OutboxStatus.Processing;
                        await context.SaveChangesAsync(stoppingToken);

                        await PublishMessageAsync(publishEndpoint, message);

                        message.Status = OutboxStatus.Processed;
                        message.ProcessedOn = DateTimeOffset.UtcNow;
                        await context.SaveChangesAsync(stoppingToken);

                        logger.LogInformation("Outbox message processed successfully: {MessageId}", message.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing outbox message: {MessageId}", message.Id);

                        message.Status = message.RetryCount >= 3 ? OutboxStatus.Dead : OutboxStatus.Retrying;
                        message.Error = ex.Message;
                        message.RetryCount++;
                        await context.SaveChangesAsync(stoppingToken);
                    }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OutboxProcessorWorker loop");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

        logger.LogInformation("OutboxProcessorWorker stopped");
    }

    private async Task PublishMessageAsync(IPublishEndpoint publishEndpoint, OutboxMessage message)
    {
        switch (message.Type)
        {
            case OutboxMessageType.ForgotPasswordEmail:
                var emailMessage = JsonSerializer.Deserialize<ForgotPasswordEmailMessage>(
                    message.Content.RootElement.GetRawText());

                if (emailMessage != null)
                {
                    await publishEndpoint.Publish(emailMessage);
                    logger.LogInformation("Published ForgotPasswordEmail message for: {Email}", emailMessage.Email);
                }
                else
                {
                    logger.LogWarning("Failed to deserialize ForgotPasswordEmailMessage for message: {MessageId}",
                        message.Id);
                }

                break;

            default:
                logger.LogWarning("Unknown message type: {Type}", message.Type);
                break;
        }
    }
}