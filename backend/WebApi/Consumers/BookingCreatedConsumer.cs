using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Contracts;
using AgendaPlus.Application.Interfaces.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AgendaPlus.WebApi.Consumers;

public class BookingCreatedConsumer(
    IEmailService emailService,
    IApplicationDbContext context,
    ILogger<BookingCreatedConsumer> logger) : IConsumer<BookingCreatedMessage>
{
    public async Task Consume(ConsumeContext<BookingCreatedMessage> contextMt)
    {
        var message = contextMt.Message;

        logger.LogInformation("Consuming booking created message for Booking ID: {BookingId}", message.BookingId);

        try
        {
            // Fetch resource name for the email
            var resource = await context.Resources
                .FirstOrDefaultAsync(r => r.Id == message.ResourceId);

            var resourceName = resource?.Name ?? "Resource";

            await emailService.SendBookingConfirmationEmailAsync(
                message.CustomerEmail,
                message.CustomerName,
                message.ReservationCode,
                message.StartDateTime,
                message.EndDateTime,
                message.TotalPrice,
                resourceName);

            logger.LogInformation("Booking confirmation email sent successfully for Booking ID: {BookingId}",
                message.BookingId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending booking confirmation email for Booking ID: {BookingId}",
                message.BookingId);
            throw; // MassTransit will automatically retry
        }
    }
}