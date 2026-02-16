using System.Net;
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

            // Get template parameters for the forgot-password email (deconstructed)
            var parameters = GetForgotPasswordEmailParameters(userName, resetToken);
            message.Subject = parameters.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildEmailHtml(parameters)
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

    private static string BuildEmailHtml(EmailTemplateParameters parameters)
    {
        // Build optional sections
        var headerText = WebUtility.HtmlEncode(parameters.Title ?? string.Empty);
        var preHeaderText = parameters.PreHeader ?? string.Empty;

        var buttonSection = string.Empty;
        if (!string.IsNullOrWhiteSpace(parameters.ButtonText) && !string.IsNullOrWhiteSpace(parameters.ButtonUrl))
        {
            var encodedText = WebUtility.HtmlEncode(parameters.ButtonText);
            var encodedUrl = WebUtility.HtmlEncode(parameters.ButtonUrl);
            buttonSection =
                $"<p style='text-align:center;'><a class='button' href='{encodedUrl}'>{encodedText}</a></p>";
        }

        // Template uses placeholders to avoid escaping many braces in CSS; we'll replace them below.
        var template = $$"""
                         <!DOCTYPE html>
                         <html>
                         <head>
                             <meta charset='UTF-8'>
                             <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                             <style>
                                 body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                                 .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                                 .header { background-color: #4CAF50; color: white; padding: 20px; text-align: center; }
                                 .content { padding: 20px; background-color: #f9f9f9; }
                                 .button { display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }
                                 .footer { text-align: center; padding: 20px; font-size: 12px; color: #777; }
                                 code.token { display: block; background: #fff; padding: 15px; border-left: 4px solid #4CAF50; word-break: break-all; }
                             </style>
                         </head>
                         <body>
                             <div class='container'>
                                 <div class='header'>
                                     <h1>AgendaPlus</h1>
                                 </div>
                                 <div class='content'>
                                     <h2>{{parameters.Title}}</h2>
                                     {{parameters.PreHeader}}

                                     {{parameters.BodyHtml}}

                                     {{buttonSection}}
                                 </div>
                                 <div class='footer'>
                                     {{parameters.FooterHtml}}
                                 </div>
                             </div>
                         </body>
                         </html>
                         """;

        // Perform replacements for placeholders
        var result = template
            .Replace("{{title}}", headerText)
            .Replace("{{preHeader}}", preHeaderText)
            .Replace("{{bodyHtml}}", parameters.BodyHtml)
            .Replace("{{buttonSection}}", buttonSection)
            .Replace("{{footerHtml}}", parameters.FooterHtml);

        return result;
    }

    private static EmailTemplateParameters GetForgotPasswordEmailParameters(string userName, string resetToken)
    {
        const string subject = "Recuperação de Senha - AgendaPlus";

        // TODO: Configurar a URL do frontend via configuração/environment variable
        const string frontendUrl = "http://localhost:3000"; // URL do frontend
        var resetUrl = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}";

        var bodyHtml = $"""
                        <h3>Olá, {WebUtility.HtmlEncode(userName)}!</h3>
                        <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
                        <p>Clique no botão abaixo para redefinir sua senha:</p>
                        <p>Este link expira em 1 hora.</p>
                        <p>Se você não solicitou esta redefinição de senha, por favor ignore este e-mail.</p>
                        """;

        const string footerHtml = "&copy; 2026 AgendaPlus. Todos os direitos reservados.";

        const string buttonText = "Redefinir Senha";

        var preHeader = string.Empty;

        return new EmailTemplateParameters(subject, bodyHtml, footerHtml, null, buttonText, resetUrl, preHeader);
    }
}

internal record EmailTemplateParameters(
    string Subject,
    string BodyHtml,
    string FooterHtml,
    string? Title,
    string? ButtonText,
    string? ButtonUrl,
    string? PreHeader);