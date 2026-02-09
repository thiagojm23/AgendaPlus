using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AgendaPlus.Infrastructure.Services;

public class EmailService(
    IOptions<SmtpSettings> smtpSettings,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task SendForgotPasswordEmailAsync(string toEmail, string userName, string resetToken)
    {
        try
        {
            logger.LogInformation("Sending forgot password email to: {Email}", toEmail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress(userName, toEmail));
            message.Subject = "Recuperação de Senha - AgendaPlus";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildForgotPasswordEmailHtml(userName, resetToken)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Forgot password email sent successfully to: {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending forgot password email to: {Email}", toEmail);
            throw;
        }
    }

    private static string BuildForgotPasswordEmailHtml(string userName, string resetToken)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>AgendaPlus</h1>
        </div>
        <div class='content'>
            <h2>Olá, {userName}!</h2>
            <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
            <p>Use o token abaixo para redefinir sua senha:</p>
            <div style='background-color: #fff; padding: 15px; border-left: 4px solid #4CAF50; margin: 20px 0;'>
                <strong>Token de Recuperação:</strong><br/>
                <code style='font-size: 14px; word-break: break-all;'>{resetToken}</code>
            </div>
            <p>Este token expira em 1 hora.</p>
            <p>Se você não solicitou esta redefinição de senha, por favor ignore este e-mail.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 AgendaPlus. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }
}