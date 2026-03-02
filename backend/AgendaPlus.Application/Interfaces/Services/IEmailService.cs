namespace AgendaPlus.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendForgotPasswordEmailAsync(string toEmail, string userName, string resetToken);

    Task SendBookingConfirmationEmailAsync(
        string toEmail,
        string customerName,
        string reservationCode,
        DateTime startDateTime,
        DateTime endDateTime,
        decimal totalPrice,
        string resourceName);
}