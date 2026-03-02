namespace AgendaPlus.Application.Interfaces.Services;

public interface ISmsService
{
    Task SendBookingConfirmationSmsAsync(string phoneNumber, string customerName, string reservationCode);
}