using AgendaPlus.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Infrastructure.Services;

public class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public Task SendBookingConfirmationSmsAsync(string phoneNumber, string customerName, string reservationCode)
    {
        // TODO: Implement integration with SMS provider (Twilio, AWS SNS, etc.)
        logger.LogInformation(
            "SMS stub - Would send booking confirmation to {PhoneNumber} for {CustomerName} with code {Code}",
            phoneNumber, customerName, reservationCode);

        return Task.CompletedTask;
    }
}