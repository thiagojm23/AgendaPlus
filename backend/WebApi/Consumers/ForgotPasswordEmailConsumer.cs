using AgendaPlus.Application.Contracts;
using AgendaPlus.Application.Interfaces.Services;
using MassTransit;

namespace AgendaPlus.WebApi.Consumers;

public class ForgotPasswordEmailConsumer(
    IEmailService emailService,
    ILogger<ForgotPasswordEmailConsumer> logger) : IConsumer<ForgotPasswordEmailMessage>
{
    public async Task Consume(ConsumeContext<ForgotPasswordEmailMessage> context)
    {
        var message = context.Message;

        logger.LogInformation("Consuming forgot password email message for: {Email}", message.Email);

        try
        {
            await emailService.SendForgotPasswordEmailAsync(message.Email, message.UserName, message.ResetToken);
            
            logger.LogInformation("Forgot password email sent successfully to: {Email}", message.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending forgot password email to: {Email}", message.Email);
            throw; // MassTransit vai fazer retry automático
        }
    }
}
