namespace AgendaPlus.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendForgotPasswordEmailAsync(string toEmail, string userName, string resetToken);
}
